using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinEvolveSSLibrary
{
    public class SystemConstants
    {
        private static bool RunLocalSS = false;
        private static bool RunLocalMySql = false;

        public static bool UseAuthentication { get { return true; } }

        public static string WebServiceDomain
        {
            get
            {
                return RunLocalSS ?
                    "localhost" :
                    "mobillho.w12.wh-2.com";
            }
        }

        public static string WebServiceBaseURL
        {
            get
            {
                return RunLocalSS ?
                    string.Format("http://{0}:54238", WebServiceDomain) :
                    string.Format("http://{0}/evolve2013", WebServiceDomain);
            }
        }

        public static string DatabaseServer
        {
            get
            {
                return RunLocalMySql ?
                    "mobillserver" :
                    "my03.winhost.com";
            }
        }

        public static string DatabaseName
        {
            get
            {
                return RunLocalMySql ?
                    "evolve_2013" :
                    "mysql_51059_evolve2013";
            }
        }

        public static string DatabaseUser
        {
            get
            {
                return RunLocalMySql ?
                    "evolve_app" :
                    "evolve2013";
            }
        }

        public static string DatabasePassword
        {
            get
            {
                return RunLocalMySql ?
                    "NLZy0qB6zHWute4CTQwf" :
                    "s30tz40c5WGM7tiK5vv3";
            }
        }

        public static string PathToRoot
        {
            get
            {
                return RunLocalSS ?
                    @"D:\Bill\Code\XamarinEvolve2013Project\XamarinEvolveSS" :
                    @"E:\web\mobillho\Evolve2013\";

            }
        }

        public static uint MaxAvatarSize { get { return 200; } }

		public static string GoogleAPIKey { get {return "AIzaSyCLphEJioicKAXQOE3NQ5gGnmPkdxaBs6o";}}
    }
}
