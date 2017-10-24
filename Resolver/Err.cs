
namespace Resolver
{
    using System;
    /// <summary>
    /// An exception class to handle the VB-style errors
    /// </summary>
    public class Err : System.Exception
    {
        //private static string _source;
        private static string _description;
        private static string _lexeme;
        private static int _linenumber;
        private static int _offset;
        //private static System.Exception error;

        //public static System.String Description
        //{
        //    get
        //    {
        //        return _description;
        //    }
        //    set
        //    {
        //        _description = value;
        //    }
        //}
        public static System.String Lexeme
        {
            get
            {
                return _lexeme;
            }
            set
            {
                _lexeme = value;
            }
        }
        public static int LineNumber
        {
            get
            {
                return _linenumber;
            }

            set
            {
                _linenumber = value;
            }
        }
        public static int Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                _offset = value;
            }
        }
        public static int HelpContext
        {
            get
            {
                return 0;
            }

            set
            {
            }

        }
        public static System.String HelpFile
        {
            get
            {
                return null;
            }

            set
            {
            }

        }
        public static int Number
        {
            get
            {
                return 0;
            }

            set
            {
            }

        }
/*        public virtual static System.String Source // overides  system.exception.source
        {
            get
            {
                return _source;
            }

            set
            {
                _source = value;
            }

        }
 */       public static void Err_Renamed_Method()
        {
        }

        // 
        // Methods
        // 
        public static void Clear()
        {
            //_source = "";
            _description = "";
            _lexeme = "";
            _linenumber = 0;
            _offset = 0;
        }

        public static void Set(System.Exception param1)
        {
        }

        public static void Set(System.Exception param1, bool param2)
        {
        }

        public static void printStackTrace()
        {
        }

        //public static void raise(int param1, System.String Source, System.String Description)
        //{
        //    //Err.Source = Source;
        //    throw new Err();
        //}

        //public static void raise(int ErrNum, System.String Source, System.String Description, System.String Lexeme, int LineNumber, int Offset)
        //{
        //    //Err.Source = Source;
        //    Err.Description = Description;
        //    Err.Lexeme = Lexeme;
        //    Err.LineNumber = LineNumber;
        //    Err.Offset = Offset;
        //    throw new Err();
        //}

        //public static void set_Renamed(System.Exception param1)
        //{
        //    throw new Err();
        //}

        //public static void set_Renamed(System.Exception param1, System.String Source)
        //{
        //    //Err.Source = Source;

        //    throw new Err();
        //}

        //public static void set_Renamed(System.Exception param1, bool param2)
        //{
        //    throw new Err();
        //}

        //public override string ToString()
        //{
        //    return "Error Description: " + Err.Description; //+ ", Source: " + Err.Source;
        //}
    }
}