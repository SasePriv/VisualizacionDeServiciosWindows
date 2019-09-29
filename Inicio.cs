using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Management;

namespace VisualizacionDeProcesos
{
    public partial class Inicio : Form
    {
        ServiceController[] servicios;
        int[] idServicios;
        string[] serviceDescription;

        public Inicio()
        {
            servicios = ServiceController.GetServices();
            idServicios = setServiceid(servicios);
            serviceDescription = setServiceDescription(servicios);
            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {

        }

        //Obtener el id de cada servicio
        private uint GetProcessIDByServiceName(string serviceName)
        {
            uint processId = 0;
            string qry = "SELECT PROCESSID FROM WIN32_SERVICE WHERE NAME = '" + serviceName + "'";
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(qry);
            foreach (System.Management.ManagementObject mngntObj in searcher.Get())
            {
                processId = (uint)mngntObj["PROCESSID"];
            }
            return processId;
        }

        //Obtener la descripcion de cada servicio
        private string GetServiceDescription(string serviceName)
        {
            using (ManagementObject service = new ManagementObject(new ManagementPath(string.Format("Win32_Service.Name='{0}'", serviceName))))
            {
                return service["Description"].ToString();
            }
        }

        //Asignando el id de todos los servicios en un vector
        private int[] setServiceid(ServiceController []service)
        {
            int[] temp = new int[] { };
            for(int i = 0; i < service.Length; i++)
            {
                temp[i] = (int)GetProcessIDByServiceName(service[i].ServiceName);
            }
            return temp;
        }

        //Asignando la descripcion de todas las descripciones en un vector
        private string[] setServiceDescription(ServiceController[] service)
        {
            string[] temp = new string[] { };
            for (int i = 0; i < service.Length; i++)
            {
                temp[i] = GetServiceDescription(service[i].ServiceName);
            }
            return temp;
        }

      
    }
}
