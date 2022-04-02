﻿using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

using NLog;

namespace WHS.DEVICE.WEIGHT.SerialPortLib
{

    /// <summary>
    /// DataBits enum.
    /// </summary>
    public enum DataBits
    {
        /// <summary>
        /// DataBits 5.
        /// </summary>
        Five = 5,
        /// <summary>
        /// DataBits 6.
        /// </summary>
        Six,
        /// <summary>
        /// DataBits 7.
        /// </summary>
        Seven,
        /// <summary>
        /// DataBits 8.
        /// </summary>
        Eight,
    }

    /// <summary>
    /// Serial port I/O
    /// </summary>
    public class SerialPortInput
    {

        #region Private Fields

        internal static Logger logger = LogManager.GetCurrentClassLogger();

        private SerialPort _serialPort;
        private string _portName = "";
        private int _baudRate = 115200;
        private StopBits _stopBits = StopBits.One;
        private Parity _parity = Parity.None;
        private DataBits _dataBits = DataBits.Eight;

        // Read/Write error state variable
        private bool gotReadWriteError = true;

        // Serial port reader task
        private Thread reader;
        // Serial port connection watcher
        private Thread connectionWatcher;

        private object accessLock = new object();
        private bool disconnectRequested = false;

        #endregion

        #region Public Events

        /// <summary>
        /// Connected state changed event.
        /// </summary>
        public delegate void ConnectionStatusChangedEventHandler(object sender, ConnectionStatusChangedEventArgs args);
        /// <summary>
        /// Occurs when connected state changed.
        /// </summary>
        public event ConnectionStatusChangedEventHandler ConnectionStatusChanged;

        /// <summary>
        /// Message received event.
        /// </summary>
        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs args);
        /// <summary>
        /// Occurs when message received.
        /// </summary>
        public event MessageReceivedEventHandler MessageReceived;

        #endregion

        #region Public Members

        /// <summary>
        /// Connect to the serial port.
        /// </summary>
        public bool Connect()
        {
            if (disconnectRequested)
                return false;
            lock (accessLock)
            {
                Disconnect();
                Open();
                connectionWatcher = new Thread(ConnectionWatcherTask);
                connectionWatcher.Start();
            }
            return IsConnected;
        }

        /// <summary>
        /// Disconnect the serial port.
        /// </summary>
        public void Disconnect()
        {
            if (disconnectRequested)
                return;
            disconnectRequested = true;
            Close();
            lock (accessLock)
            {
                if (connectionWatcher != null)
                {
                    if (!connectionWatcher.Join(5000))
                        connectionWatcher.Abort();
                    connectionWatcher = null;
                }
                disconnectRequested = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the serial port is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get { return _serialPort != null && !gotReadWriteError && !disconnectRequested; }
        }

        /// <summary>
        /// Sets the serial port options.
        /// </summary>
        /// <param name="portName">Portname.</param>
        /// <param name="baudRate">Baudrate.</param>
        /// <param name="stopBits">Stopbits.</param>
        /// <param name="parity">Parity.</param>
        /// <param name="dataBits">Databits.</param>
        public void SetPort(string portName, int baudRate = 115200, StopBits stopBits = StopBits.One, Parity parity = Parity.None, DataBits dataBits = DataBits.Eight)
        {
            if (_portName != portName)
            {
                // set to error so that the connection watcher will reconnect
                // using the new port
                gotReadWriteError = true;
            }
            _portName = portName;
            _baudRate = baudRate;
            _stopBits = stopBits;
            _parity = parity;
            _dataBits = dataBits;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <returns><c>true</c>, if message was sent, <c>false</c> otherwise.</returns>
        /// <param name="message">Message.</param>
        public bool SendMessage(byte[] message)
        {
            bool success = false;
            if (IsConnected)
            {
                try
                {
                    _serialPort.Write(message, 0, message.Length);
                    success = true;
                    logger.Debug(BitConverter.ToString(message));
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
            return success;
        }

        #endregion

        #region Private members

        #region Serial Port handling

        private bool Open()
        {
            bool success = false;
            lock (accessLock)
            {
                Close();
                try
                {
                    bool tryOpen = true;
                    if (Environment.OSVersion.Platform.ToString().StartsWith("Win") == false)
                    {
                        tryOpen = (tryOpen && System.IO.File.Exists(_portName));
                    }
                    if (tryOpen)
                    {
                        _serialPort = new SerialPort();
                        _serialPort.ErrorReceived += HandleErrorReceived;
                        _serialPort.PortName = _portName;
                        _serialPort.BaudRate = _baudRate;
                        _serialPort.StopBits = _stopBits;
                        _serialPort.Parity = _parity;
                        _serialPort.DataBits = (int)_dataBits;

                        // We are not using serialPort.DataReceived event for receiving data since this is not working under Linux/Mono.
                        // We use the readerTask instead (see below).
                        _serialPort.Open();
                        success = true;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    Close();
                }
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    gotReadWriteError = false;
                    // Start the Reader task
                    reader = new Thread(ReaderTask);
                    reader.Start();
                    OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(true));
                }
            }
            return success;
        }

        private void Close()
        {
            lock (accessLock)
            {
                // Stop the Reader task
                if (reader != null)
                {
                    if (!reader.Join(5000))
                        reader.Abort();
                    reader = null;
                }
                if (_serialPort != null)
                {
                    _serialPort.ErrorReceived -= HandleErrorReceived;
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(false));
                    }
                    _serialPort.Dispose();
                    _serialPort = null;
                }
                gotReadWriteError = true;
            }
        }

        private void HandleErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            logger.Error(e.EventType);
        }

        #endregion

        #region Background Tasks

        private void ReaderTask()
        {
            while (IsConnected)
            {
                int msglen = 0;
                //
                try
                {
                    msglen = _serialPort.BytesToRead;
                    if (msglen > 0)
                    {
                        byte[] message = new byte[msglen];
                        //
                        int readbytes = 0;
                        while (_serialPort.Read(message, readbytes, msglen - readbytes) <= 0)
                            ; // noop
                        if (MessageReceived != null)
                        {
                            OnMessageReceived(new MessageReceivedEventArgs(message));
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    gotReadWriteError = true;
                    Thread.Sleep(1000);
                }
            }
        }

        private void ConnectionWatcherTask()
        {
            // This task takes care of automatically reconnecting the interface
            // when the connection is drop or if an I/O error occurs
            while (!disconnectRequested)
            {
                if (gotReadWriteError)
                {
                    try
                    {
                        Close();
                        // wait 1 sec before reconnecting
                        Thread.Sleep(1000);
                        if (!disconnectRequested)
                        {
                            try
                            {
                                Open();
                            }
                            catch (Exception e)
                            {
                                logger.Error(e);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
                }
                if (!disconnectRequested)
                    Thread.Sleep(1000);
            }
        }

        #endregion

        #region Events Raising

        /// <summary>
        /// Raises the connected state changed event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnConnectionStatusChanged(ConnectionStatusChangedEventArgs args)
        {
            logger.Debug(args.Connected);
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, args);
        }

        /// <summary>
        /// Raises the message received event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
        {
            logger.Debug(BitConverter.ToString(args.Data));
            if (MessageReceived != null)
                MessageReceived(this, args);
        }

        #endregion

        #endregion

    }

}
