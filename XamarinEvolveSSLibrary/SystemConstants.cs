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

        //public const string WebServiceBaseURL = "http://mobillho.w12.wh-2.com/evolve2013";
        //public const string DatabaseServer = "my03.winhost.com";
        //public const string DatabaseName = "mysql_51059_evolve2013";
        //public const string DatabaseUser = "evolve2013";
        //public const string DatabasePassword = "s30tz40c5WGM7tiK5vv3";

        //public const string WebServiceBaseURL = "http://localhost:54238";
        //public const string WebServiceBaseURL = "http://mobillho.w12.wh-2.com/evolve2013";
        //public const string DatabaseName = "evolve_2013";
        //public const string DatabaseServer = "mobillserver";
        //public const string DatabaseUser = "evolve_app";
        //public const string DatabasePassword = "NLZy0qB6zHWute4CTQwf";

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
    }
}
