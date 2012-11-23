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

        public static string WebServiceBaseURL
        {
            get
            {
                return RunLocalSS ?
                    "http://localhost:54238" :
                    "http://mobillho.w12.wh-2.com/evolve2013";
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
    }
}
