using System;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;

namespace WHS_CustomAction
{
    public class CustomAction
    {
        [CustomAction]
        public static ActionResult UpdateFiles(Session session)
        {
            session.Log("开始执行自定义安装动作");

            //检查是否存在 C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App

            TryCopy(Resource.RCSMap, @"D:\RCSMap.map", true, session);

            return ActionResult.Success;
        }

        private static void TryCopy(byte[] source, string target, bool overwrite, Session session)
        {
            try
            {
                File.WriteAllBytes(target, source);
                session.Log("成功");

            }
            catch (Exception ex)
            {
                session.Log("exception  " + ex.Message);
            }
        }

        private static void TryCopy(string source, string target, bool overwrite, Session session)
        {
            try
            {
                File.WriteAllText(target, source);
                session.Log("成功");

            }
            catch (Exception ex)
            {
                session.Log("exception  " + ex.Message);
            }
        }
    }
}
