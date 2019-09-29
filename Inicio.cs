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
        string selectedService;

        public Inicio()
        {
            iniviar_valores();
            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            renderProcessesOnListView(servicios, idServicios, serviceDescription);
            listView1.FullRowSelect = true;
            btn_pause.Enabled = false;
            btn_resume.Enabled = false;
            btn_start.Enabled = false;
            btn_stop.Enabled = false;           
        }     
       
        //inicializa los valores
        private void iniviar_valores()
        {
            servicios = ServiceController.GetServices();
            idServicios = setServiceid(servicios);
            serviceDescription = setServiceDescription(servicios);
        }
        //Hace la actualizacion de la info del listview
        private void updateApplications()
        {
            listView1.Items.Clear();
            iniviar_valores();
            renderProcessesOnListView(servicios, idServicios, serviceDescription);
        }

        public void renderProcessesOnListView(ServiceController[] servicios ,int[] idServicios,string[] serviceDescription)
        {
            // Loop through the array of processes to show information of every process in your console
            for(int i = 0; i < servicios.Length; i++)
            {
                // Create an array of string that will store the information to display in our 
                string[] row = {
                    // 1 Servicio name
                    servicios[i].DisplayName,
                    // 2 Servicio ID
                    idServicios[i].ToString(),
                    // 3 Servicio status
                    servicios[i].Status.ToString(),
                    // 6 Description of the servicio
                    serviceDescription[i]
                };

                // Create a new Item to add into the list view that expects the row of information as first argument
                ListViewItem item = new ListViewItem(row);
                // Add the Item
                listView1.Items.Add(item);
            }
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
            try
            {
                using (ManagementObject service = new ManagementObject(new ManagementPath(string.Format("Win32_Service.Name='{0}'", serviceName))))
                {
                    return service["Description"].ToString();
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

        //Asignando el id de todos los servicios en un vector
        private int[] setServiceid(ServiceController []service)
        {
            int[] temp = new int[service.Length];
            
            for(int i = 0; i < service.Length; i++)
            {
                temp[i] = (int)GetProcessIDByServiceName(service[i].ServiceName);
            }
            return temp;
        }

        //Asignando la descripcion de todas las descripciones en un vector
        private string[] setServiceDescription(ServiceController[] service)
        {
            string[] temp = new string[service.Length];
            for (int i = 0; i < service.Length; i++)
            {
                temp[i] = GetServiceDescription(service[i].ServiceName);
            }
            return temp;
        }

        //Obtener el codigo del status de los botones
        private String GetBottonStatus(String servicename)
        {
            string resp;
            ServiceController sc = new ServiceController(servicename);
            if (sc.CanStop)
            {
                resp = "0,1,0,0";
                if (sc.CanPauseAndContinue)
                {
                    if(sc.Status.ToString() == "Paused") 
                    {
                        resp = "0,1,0,1";
                    }
                    else
                    {
                        resp = "0,1,1,0";
                    }
                }
            }
            else
            {
                resp = "1,0,0,0";
            }
            return resp;
        }

        //al hacer click en la tabla abilita los botones si es que se puede
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            selectedService = listView1.SelectedItems[0].SubItems[0].Text;            
            string cod = GetBottonStatus(selectedService);
            string[] resp = cod.Split(',');
            Console.WriteLine(resp[2]);
            
            //Boton de start
            if (resp[0] == "1")
            {
                btn_start.Enabled = true;                
            }
            else
            {
                btn_start.Enabled = false;
            }
            //Boton de stop
            if(resp[1] == "1")
            {
                btn_stop.Enabled = true;
            }
            else
            {
                btn_stop.Enabled = false;
            }
            //Boton de pause
            if(resp[2] == "1")
            {
                btn_pause.Enabled = true;
            }
            else
            {
                btn_pause.Enabled = false;
            }
            //Boton resume
            if(resp[3] == "1")
            {
                btn_resume.Enabled = true;
            }
            else
            {
                btn_resume.Enabled = false;
            }


        }

        //Eventos botones
        private void btn_start_Click(object sender, EventArgs e)
        {
            ServiceController sr = new ServiceController(selectedService);
            sr.Start();
            updateApplications();
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            ServiceController sr = new ServiceController(selectedService);
            sr.Stop();
            updateApplications();
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            ServiceController sr = new ServiceController(selectedService);
            sr.Pause();
            updateApplications();
        }

        private void btn_resume_Click(object sender, EventArgs e)
        {
            ServiceController sr = new ServiceController(selectedService);
            sr.Continue();
            updateApplications();
        }
    }
}
