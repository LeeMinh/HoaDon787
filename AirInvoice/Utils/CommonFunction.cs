using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AirlineInvoice.Utils
{
    public class CommonFunction
    {

        public static bool CheckAgentUserPermission(string userName = "")
        {
            var current  = Models.userprofile.CurrentUser;
            if (current !=null && string.IsNullOrEmpty(userName))
            {
                userName = current.UserName;
            }
            var user = Models.userprofile.GetUser(userName);
            if ((user != null && user.AgentBranchID <= 0) || Roles.IsUserInRole(_AdminRole))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public const string _AdminRole = "Admin";
        public const string _SuperAdmin = "SuperAdmin";
        public const string _Accounting = "Accounting";
        public const string _OperatorRole = "Operator";
        public static Dictionary<string, string> RoleMapping = new Dictionary<string, string>() { 
        {_SuperAdmin,"Quản trị cao nhất"},
        {_AdminRole,"Quản trị"},
        {_OperatorRole,"Báo cáo"},
        {_Accounting,"Kế toán"}
        };
        public static Dictionary<int, double> MaximumCarLoading = new Dictionary<int, double>() { 
        {1,10000 }, 
        {2,16000 },
         {3,24000 },
         {4,30000 },
         {5,34000 },
         {6,34000 },
         {7,34000 },
         {8,34000 },
         {9,34000 },
         {10,34000 }
        };

        /// <summary>
        /// Lay khach san ma nhan vien le tan dang dang nhap phu trach
        /// </summary>
        /// <returns></returns>
        public static int GetAgent()
        {
            if (Membership.GetUser() == null) return -1;
            var user = Models.userprofile.CurrentUser;
            if (user != null)
            {
                return user.AgentID;
            }
            else
            {
                return -1;
            }
        }

        public static List<string> ListModulesMonitor = new List<string>() {
         {"hotel"},{"room"}, {"customer"}
        };
        public static string GetUserFullName(string userID="")
        {
            var mem = string.IsNullOrEmpty(userID) ? Membership.GetUser() : Membership.GetUser(new Guid(userID));
            string fullName = string.Empty;
            if (mem != null)
            {
                var user = Models.userprofile.GetUser(mem.UserName);
                if (user != null)
                {
                    fullName = Membership.GetUser().UserName;
                    if (user != null)
                    {
                        if (!user.UserName.Equals("admin"))
                        {
                            fullName = user.FullName;
                        }
                    }
                }
            }
            return fullName;
        }


        public static string GetUsernameByID(string userID = "")
        {
            var mem = string.IsNullOrEmpty(userID) ? Membership.GetUser() : Membership.GetUser(new Guid(userID));
            string fullName = string.Empty;
            if (mem != null)
            {
                var user = Models.userprofile.GetUser(mem.UserName);
                if (user != null)
                {
                    fullName = user.UserName;
                }
            }
            return fullName;
        }


        public static string GetUserAvatar()
        {
            var user = Models.userprofile.CurrentUser;
            string avatar = string.Empty;
            if (user != null)
            {
                avatar = user.AvatarLink;
                if (string.IsNullOrEmpty(avatar))
                {
                    avatar = Guid.NewGuid().ToString();
                }
            }
            return "~/images/" + avatar + ".jpg";
        }

        public static string EncryptString(string InputText)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);
            byte[] Salt = System.Text.Encoding.ASCII.GetBytes(Properties.Resources.Cryptography_Salt.Length.ToString());

            //This class uses an extension of the PBKDF1 algorithm defined in the PKCS#5 v2.0 
            //standard to derive bytes suitable for use as key material from a password. 
            //The standard is documented in IETF RRC 2898.

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Properties.Resources.Cryptography_Salt, Salt);
            //Creates a symmetric encryptor object. 
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            //Defines a stream that links data streams to cryptographic transformations
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(PlainText, 0, PlainText.Length);
            //Writes the final state and clears the buffer
            cryptoStream.FlushFinalBlock();
            byte[] CipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string EncryptedData = Convert.ToBase64String(CipherBytes);
            return EncryptedData;

        } //eof private static string EncryptString ( string InputText, string Password )

        /// <summary>
        /// Method which does the encryption using Rijndeal algorithm.This is for decrypting the data
        /// which has orginally being encrypted using the above method
        /// </summary>
        /// <param name="InputText">The encrypted data which has to be decrypted</param>
        /// <param name="Password">The string which has been used for encrypting.The same string
        /// should be used for making the decrypt key</param>
        /// <returns>Decrypted Data</returns>
        public static string DecryptString(string InputText)
        {

            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            byte[] EncryptedData = Convert.FromBase64String(InputText);
            byte[] Salt = System.Text.Encoding.ASCII.GetBytes(Properties.Resources.Cryptography_Salt.Length.ToString());
            //Making of the key for decryption
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Properties.Resources.Cryptography_Salt, Salt);
            //Creates a symmetric Rijndael decryptor object.
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(EncryptedData);
            //Defines the cryptographics stream for decryption.THe stream contains decrpted data
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] PlainText = new byte[EncryptedData.Length];
            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();
            //Converting to string
            string DecryptedData = System.Text.Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            return DecryptedData;

        }

        /// <summary>
        /// Converts a Base-10 Integer to Base-64.  This will effectively shorten the number of characters used to represent a number > 10.  
        /// </summary>
        public static string Shrink(int Number)
        {
            try
            {
                return ConvertDecToBase(Number, 64);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return "";
            }

        }
        public static void LogError(Exception ex)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)
            if (stackFrames.Count() > 1) log4net.LogManager.GetLogger(stackFrames[1].GetMethod().DeclaringType).Error(ex);

        }
        /// <summary>
        /// Converts a Base-64 Number (represented as a string) into a Base-10 Integer.  This will effectively expand the numbers of characters needed to represent the number.
        /// </summary>
        /// <param name="NumberString"></param>
        /// <returns></returns>
        public static int Expand(string NumberString)
        {
            if (string.IsNullOrEmpty(NumberString)) return 0;
            return ConvertBaseToDec(NumberString, 64);
        }

        private static string DefaultChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

        private static int ConvertBaseToDec(string NumberString, int Base)
        {
            try
            {
                string Chars;
                if (Base == 64)
                {
                    NumberString = Regex.Replace(NumberString, "^A+", "");
                    Chars = Base64Chars;
                }
                else
                {
                    NumberString = Regex.Replace(NumberString, "^0+", "");
                    Chars = DefaultChars;
                }

                int Dec = 0;
                for (int i = 0; i <= NumberString.Length - 1; i++)
                {
                    int CharNum = Chars.IndexOf(NumberString[i]);
                    int Conversion = CharNum * IntPow(Base, (NumberString.Length - (i + 1)));
                    Dec = Dec + Conversion;
                }

                return Dec;
            }
            catch
            {
                return -1;
            }
        }

        private static string ConvertDecToBase(int OriginalNumber, int Base)
        {
            char[] Chars;
            if (Base == 64)
            {
                Chars = Base64Chars.ToCharArray();
            }
            else
            {
                Chars = DefaultChars.ToCharArray();
            }

            string encoded = "";
            encoded = ConvertDecToBase(OriginalNumber, Base, encoded, Chars);
            return encoded;
        }

        private static string ConvertDecToBase(int Number, int Base, string EncodedString, char[] Chars)
        {
            if (Number < Base)
            {
                EncodedString = EncodedString + Chars[Number];
            }
            else
            {
                int NewNumber = Number / Base;
                EncodedString = ConvertDecToBase(NewNumber, Base, EncodedString, Chars);

                Number = Number - (NewNumber * Base);
                if (Number < Base)
                {
                    EncodedString = EncodedString + Chars[Number];
                }
            }
            return EncodedString;
        }

        private static int IntPow(int x, int pow)
        {
            int ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        /// <summary>
        /// Chuyển Partial View thành HTML
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");
            var viewData = new ViewDataDictionary(model);
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public static string CombinePath(string dir1, string dir2)
        {
            if (!dir1.EndsWith("\\"))
                dir1 = dir1 + "\\";

            if (dir2.StartsWith("\\"))
                dir2 = dir2.Substring(1);
            return dir1 + dir2;
        }

        public static string WebCombinePath(string dir1, string dir2)
        {
            if (dir1 == null) dir1 = string.Empty;
            if (dir2 == null) dir2 = string.Empty;
            if (!dir1.EndsWith("/"))
                dir1 = dir1 + "/";

            if (dir2.StartsWith("/"))
                dir2 = dir2.Substring(1);
            return dir1 + dir2;
        }


        public static Dictionary<string, double> MaximumAxleLoading = new Dictionary<string, double>() { 
         {"1",10000 },
         {"2_d<1",11000 },
         {"2_1<=d<1,3",16000 },
         {"2_d>=1,3",18000 },
         {"3_d<=1,3",21000 },
         {"3_d>1,3",24000 }
        };
        public static string GetRefOverload(int totalAxles, double Distance)
        {
            string result = string.Empty;
            if (totalAxles == 1)
            {
                result = "1";
            }
            else if (totalAxles == 2)
            {
                if (Distance < 1.1)
                {
                    result = "2_d<1";
                }
                else if (Distance >= 1 && Distance < 1.3)
                {
                    result = "2_1<=d<1,3";
                }
                else
                {
                    result = "2_d>=1,3";
                }
            }
            else
            {
                if (Distance <= 1.3)
                {
                    result = "3_d<=1,3";
                }
                else
                {
                    result = "3_d>1,3";
                }
            }
            return result;
        }

        private static System.Collections.Hashtable QuarterConvert = new System.Collections.Hashtable()
        {
            { 1, "I" },
            { 2, "II"},
            {3 , "III"},
            {4, "IV"}
        };
        public static string SayDate(DateTime fromDate, DateTime toDate)
        {
            var sDate = string.Empty;
            var sMonthFormat = "Tháng {0} năm {1}";
            var sYearFormat = "Năm {0}";
            var sQuarterFormat = "Quý {0} năm {1}";
            var sPeriodFormat = "Từ {0:dd/MM/yyyy} đến {1:dd/MM/yyyy}";
            var sDayFormat = "Ngày {0:dd/MM/yyyy}";
            // Trường hợp in tháng
            if (fromDate.Month == toDate.Month && fromDate.Day == 1 && toDate.Day == DateTime.DaysInMonth(toDate.Year, toDate.Month))
            {
                sDate = string.Format(sMonthFormat, fromDate.Month, fromDate.Year);
            }
            // trường hợp in quý 
            else if (toDate.Month == fromDate.Month + 2 && fromDate.Month % 3 == 1
                && fromDate.Day == 1 && toDate.Day == DateTime.DaysInMonth(toDate.Year, toDate.Month))
            {
                sDate = string.Format(sQuarterFormat, QuarterConvert[(toDate.Month - 1) / 3 + 1], fromDate.Year);
            }
            // In cả năm
            else if (fromDate.Month == 1 && fromDate.Day == 1 && toDate.Month == 12 && toDate.Day == 31)
            {
                sDate = string.Format(sYearFormat, fromDate.Year);
            }
            // Trường hợp in một ngày
            else if ((toDate.Date - fromDate.Date).TotalMilliseconds == 0)
            {
                sDate = string.Format(sDayFormat, fromDate);
            }
            // In từ ngày đến ngày
            else
            {
                sDate = string.Format(sPeriodFormat, fromDate, toDate);
            }
            return sDate;
        }
        public static int RoundToInteger(object value, bool roundUp = false)
        {
            var result = Math.Floor(value.ConvertToDecimal()).ConvertToInt();
            if (roundUp)
            {
                if (result < value.ConvertToDecimal())
                {
                    result++;
                }
            }
            return result;
        }
        public static string NumberToString(int so)
        {
            string kq = "";
            switch (so)
            {
                case 0: kq = "không"; break;
                case 1: kq = "một"; break;
                case 2: kq = "hai"; break;
                case 3: kq = "ba"; break;
                case 4: kq = "bốn"; break;
                case 5: kq = "năm"; break;
                case 6: kq = "sáu"; break;
                case 7: kq = "bảy"; break;
                case 8: kq = "tám"; break;
                case 9: kq = "chín"; break;
            }
            return kq;
        }
        public static string SayMoney(double num)
        {
            string sNum = num.ToString(), temp = "";
            int len = sNum.Length, nhomso;
            string str = "";
            int i = 1;
            while (i <= len)
            {
                str = str + " " + NumberToString(int.Parse(sNum.Substring(i - 1, 1)));
                nhomso = (int)((len - i) % 9);
                if (i == len) break;
                if (nhomso == 0)
                {
                    str += " tỷ";
                    for (int j = 0; j < 3; j++)
                    {
                        temp = sNum.Substring(i, 3);
                        if (temp == "000")
                            i += 3;
                    }

                }
                else
                    if (nhomso == 6)
                    {
                        str += " triệu";
                        for (int j = 0; j < 2; j++)
                        {
                            temp = sNum.Substring(i, 3);
                            if (temp == "000")
                                i += 3;
                        }
                    }
                    else
                        if (nhomso == 3)
                        {
                            str += " nghìn";
                            temp = sNum.Substring(i, 3);
                            if (temp == "000")
                                i += 3;
                        }
                        else
                        {
                            nhomso = (int)((len - i) % 3);
                            if (nhomso == 2)
                                str += " trăm";
                            else
                                if (nhomso == 1)
                                    str += " mươi";
                        }
                i++;
            }
            //Đoạn này là để thay thế các câu cho phù hợp với cách đọc của người Việt Nam thôi.
            str = str.Replace("không mươi không", "");
            str = str.Replace("không mươi", "linh");
            str = str.Replace("mươi không", "mươi");
            str = str.Replace("một mươi", "mười");
            str = str.Replace("mươi bốn", "mươi tư");
            str = str.Replace("linh bốn", "linh tư");
            str = str.Replace("mươi một", "mươi mốt");
            str = str.Replace("mươi năm", "mươi lăm");
            str = str.Replace("mười năm", "mười lăm");
            if (string.IsNullOrEmpty(str)) return string.Empty;
            str = str.Trim();
            return str.First().ToString().ToUpper() + str.Substring(1) + " đồng chẵn";
        }
        public static string SayMoney(int num)
        {
            string sNum = num.ToString(), temp = "";
            int len = sNum.Length, nhomso;
            string str = "";
            int i = 1;
            while (i <= len)
            {
                str = str + " " + NumberToString(int.Parse(sNum.Substring(i - 1, 1)));
                nhomso = (int)((len - i) % 9);
                if (i == len) break;
                if (nhomso == 0)
                {
                    str += " tỷ";
                    for (int j = 0; j < 3; j++)
                    {
                        temp = sNum.Substring(i, 3);
                        if (temp == "000")
                            i += 3;
                    }

                }
                else
                    if (nhomso == 6)
                    {
                        str += " triệu";
                        for (int j = 0; j < 2; j++)
                        {
                            temp = sNum.Substring(i, 3);
                            if (temp == "000")
                                i += 3;
                        }
                    }
                    else
                        if (nhomso == 3)
                        {
                            str += " nghìn";
                            temp = sNum.Substring(i, 3);
                            if (temp == "000")
                                i += 3;
                        }
                        else
                        {
                            nhomso = (int)((len - i) % 3);
                            if (nhomso == 2)
                                str += " trăm";
                            else
                                if (nhomso == 1)
                                    str += " mươi";
                        }
                i++;
            }
            //Đoạn này là để thay thế các câu cho phù hợp với cách đọc của người Việt Nam thôi.
            str = str.Replace("không mươi không", "");
            str = str.Replace("không mươi", "linh");
            str = str.Replace("mươi không", "mươi");
            str = str.Replace("một mươi", "mười");
            str = str.Replace("mươi bốn", "mươi tư");
            str = str.Replace("linh bốn", "linh tư");
            str = str.Replace("mươi một", "mươi mốt");
            str = str.Replace("mươi năm", "mươi lăm");
            str = str.Replace("mười năm", "mười lăm");
            if (string.IsNullOrEmpty(str)) return string.Empty;
            str = str.Trim();
            return str.First().ToString().ToUpper() + str.Substring(1) + " đồng chẵn";
        }
        public static List<int> GetBitNo(int? value)
        {
            if (!value.HasValue) value = 0;
            var lst = new List<int>();
            var n = 14;
            while (n >= 0)
            {
                var x = Convert.ToInt32(Math.Pow(2, n));
                if ((value & x) == x)
                {
                    lst.Add(n);
                    value = value - Convert.ToInt32(Math.Pow(2, n));
                }
                n--;
            }
            return lst;
        }

        /// <summary>
        /// Lay gia tri config trong database
        /// </summary>
        /// <param name="OptionID"></param>
        /// <returns></returns>
        public static string GetDBOption(string OptionID)
        {
            using (var db = new Models.InvoiceContext())
            {
                string result = string.Empty;
                var obj = db.DBOptions.FirstOrDefault(x => x.OptionID.Equals(OptionID));
                if (obj != null)
                {
                    result = obj.OptionValue;
                }
                return result;
            }
        }

        public static string GetRentTypeName(byte rentType)
        {
            if (rentType == (int)RentType.OverNight)
            {
                return "Qua đêm";
            }
            else if (rentType == (int)RentType.HourStep)
            {
                return "Theo giờ";
            }
            else
                return "Theo ngày";
        }
        public static string ParseERREInvoiceVNPT(string ERR)
        {
            string messageERR = "";
            switch (ERR)
            {
                case "ERR:-3": messageERR = "Có lỗi trong quá trình lấy chứng thư"; break;
                case "ERR:-2": messageERR = "Chứng thư không có privatekey"; break;
                case "ERR:-1": messageERR = "Ấn nút hủy khi nhập mã pin của chứng thư"; break;
                case "ERR:1": messageERR = "không có quyền truy cập webservice"; break;
                case "ERR:2": messageERR = "Không tồn tại hóa đơn cần thay thế/điều chỉnh"; break;
                case "ERR:3": messageERR = "định dạng file xml hóa đơn k đúng"; break;
                case "ERR:4": messageERR = "token hóa đơn sai định dạng"; break;
                case "ERR:5": messageERR = "có lỗi xảy ra"; break;
                case "ERR:6": messageERR = "không còn đủ số hóa đơn cho lô phát hành"; break;
                case "ERR:7": messageERR = "không tìm thấy chứng thư trong máy.Hãy cắm token"; break;
                case "ERR:8": messageERR = " hoa don da duoc dieu chinh, thay the roi"; break;
                case "ERR:10": messageERR = "số lượng hóa đơn truyền vào lớn hơn maxBlockInv"; break;
                case "ERR:19": messageERR = "pattern truyen vao khong giong voi pattern cua hoa don can dieu chinh / thay the"; break;
                case "ERR:20": messageERR = "tham số mẫu số và ký hiệu truyền vào không hợp lệ"; break;
                case "ERR:21": messageERR = "không tìm thấy công ty trên hệ thống"; break;
                case "ERR:22": messageERR = "công ty chưa đăng ký thông tin keystore"; break;
                case "ERR:23": messageERR = "chung thu truyen len k dung dinh dang"; break;
                case "ERR:24": messageERR = "chứng thư truyền lên không đúng với chứng thư công ty đăng ký trên hệ thống"; break;
                case "ERR:26": messageERR = "Chứng thư đã hết hạn!"; break;
                case "ERR:27": messageERR = "Chứng thư chưa đến thời điểm sử dụng!"; break;
                case "ERR:28": messageERR = "thông tin chứng thư chưa có trong hệ thống"; break;
                case "ERR:30": messageERR = "tạo mới hóa đơn lỗi"; break;
                case "ERR:0": messageERR = "tạo mới hóa đơn lỗi do fkey trùng"; break;

            }
            return messageERR;
        }
        public static string xmlInvData = "<![CDATA[<Invoices>\r\n<Inv>\r\n<key>[key]</key>\r\n<Invoice>\r\n<CusCode>[CusCode]</CusCode>\r\n<CusName>[CusName]</CusName>\r\n<Buyer>[Buyer]</Buyer>\r\n<CusAddress>[CusAddress]</CusAddress>\r\n<CusPhone>[CusPhone]</CusPhone>\r\n<CusTaxCode>[CusTaxCode]</CusTaxCode>\r\n<PaymentMethod>[PaymentMethod]</PaymentMethod>\r\n<KindOfService>[KindOfService]</KindOfService>\r\n<Products>[Products]</Products>\r\n<Total>[Total]</Total>\r\n<Extra7>[Extra7]</Extra7>\r\n<DiscountAmount>[DiscountAmount]</DiscountAmount>\r\n<Extra>[Extra]</Extra>\r\n<Extra1>[Extra1]</Extra1>\r\n<Extra2>[Extra2]</Extra2>\r\n<Extra4>[Extra4]</Extra4>\r\n<Extra5>[Extra5]</Extra5>\r\n<Extra6>[Extra6]</Extra6>\r\n<VATAmount>[VATAmount]</VATAmount>\r\n<Amount>[Amount]</Amount>\r\n<AmountInWords>[AmountInWords]</AmountInWords>\r\n<ArisingDate>[ArisingDate]</ArisingDate>\r\n<PaymentStatus>[PaymentStatus]</PaymentStatus>\r\n</Invoice>\r\n</Inv>\r\n</Invoices>]]>";
    }
    
    public class GroupAxles
    {

        public GroupAxles()
        {
            GroupId = 0;
            Weight = 0;
            TotalAxles = 0;
            Distance = 0;
        }
        public int GroupId { get; set; } // ID của nhóm trục
        public double Weight { get; set; } // Trọng lượng nhóm trục
        public int TotalAxles { get; set; }  // Số trục trong nhóm trục
        public double Distance { get; set; } // khoảng cách 2 tâm trục kế tiếp
    }

    public partial class ExcelReport
    {
        public static void ToExcel(DataTable data, string ExceptColumn, string fileName, string titleReport, HttpResponse response)
        {
            //Loại bỏ cột Except
                if (!string.IsNullOrEmpty(ExceptColumn))
                {
                    string[] arrColumns = ExceptColumn.Split(',');
                    foreach (string col in arrColumns)
                    {
                        data.Columns.Remove(col);
                    }
                }
                var sl = new SLDocument();
                SLFont redunderline = sl.CreateFont();
                redunderline.SetFont(FontSchemeValues.Minor, 18);
                redunderline.FontColor = System.Drawing.Color.DodgerBlue;
                const int iStartRowIndex = 3;
                const int iStartColumnIndex = 2;
                sl.ImportDataTable(iStartRowIndex, iStartColumnIndex, data, true);
                SLStyle style = sl.CreateStyle();
                int numCol = 0;
                //Kiểm tra datatype column
                foreach (DataColumn col in data.Columns)
                {
                    numCol += 1;
                    if (col.DataType == typeof(DateTime))
                    {
                        style.FormatCode = "dd/mm/yyyy hh:mm:ss";
                        style.SetWrapText(true);
                        sl.SetColumnStyle(col.Ordinal + iStartColumnIndex, style);
                    }

                }
                style = sl.CreateStyle();
                style.SetWrapText(true);
                sl.SetColumnStyle(1, numCol + 1, style);

                int iEndRowIndex = iStartRowIndex + data.Rows.Count + 1 - 1;
                int iEndColumnIndex = iStartColumnIndex + data.Columns.Count - 1;
                SLTable table = sl.CreateTable(iStartRowIndex, iStartColumnIndex, iEndRowIndex, iEndColumnIndex);
                sl.AutoFitColumn(iStartColumnIndex, iEndColumnIndex, 50);
                table.SetTableStyle(SLTableStyleTypeValues.Medium4);
                //table.HasTotalRow = true;
                // table.SetTotalRowFunction(5, SLTotalsRowFunctionValues.Sum);
                sl.InsertTable(table);
                MemoryStream ms = new MemoryStream();
                sl.MergeWorksheetCells("B2", "Q2"); // Trộn ô để tiêu đề báo cáo nằm trong khoảng cột báo cáo
                style = sl.CreateStyle();
                style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                style.Font.FontName = "Times New Roman";
                style.Font.FontSize = 20;
                style.Font.FontColor = System.Drawing.Color.DeepSkyBlue;
                style.Font.Bold = true;
                style.SetWrapText(true);
                sl.SetCellValue("B2", titleReport);
                sl.SetCellStyle("B2", style);
                sl.SetRowHeight(2, 40);
                sl.SaveAs(ms);
                ms.Position = 0;
                string attachment = string.Format("attachment; filename={0}.xlsx", fileName);
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                //HttpContext.Current.Response.End();
        }

        
    }
}
namespace AirlineInvoice
{
    public static class DataUtility
    {
        public static DateTime ConvertToDateTime(this object obj)
        {
            if (obj == null || obj == DBNull.Value || string.IsNullOrEmpty(obj.ToString())) return DateTime.MinValue;
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public static Int32 ConvertToInt(this object obj, int defaultValue = 0)
        {
            if (obj == DBNull.Value) return defaultValue;
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            { return defaultValue; }
        }

        public static decimal ConvertToDecimal(this object obj, int defaultValue = 0)
        {
            if (obj == DBNull.Value) return defaultValue;
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            { return defaultValue; }
        }

        public static double ConvertToDouble(this object obj, int defaultValue = 0)
        {
            if (obj == null || obj == DBNull.Value || string.IsNullOrEmpty(obj.ToString())) return defaultValue;
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            { return defaultValue; }
        } 
    }
}