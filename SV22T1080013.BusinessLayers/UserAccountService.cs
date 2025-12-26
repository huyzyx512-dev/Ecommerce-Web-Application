using SV22T1080013.DataLayers;

namespace SV22T1080013.BusinessLayers
{
    public class UserAccountService
    {
        public static readonly EmployeeUserAccountDAL employeeUserAccountDB;
        public static readonly CustomerDAL customerUserAccountDB;

        static UserAccountService() {
            employeeUserAccountDB = new EmployeeUserAccountDAL(Configuration.ConnectionString);
            customerUserAccountDB = new CustomerDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// Giao tiếp với dữ liệu tài khoản của nhân viên
        /// </summary>
        public static EmployeeUserAccountDAL EmployeeUserAccountDB => employeeUserAccountDB;

        /// <summary>
        /// Giao tiếp với dữ liệu tài khoản của khách hàng
        /// </summary>
        public static CustomerDAL CustomerUserAccountDB => customerUserAccountDB;

    }
}
