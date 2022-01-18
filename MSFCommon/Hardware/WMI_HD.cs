/// <summary>
/// Copyright(c) 2008-2009��������ʱ��Ͷ�ʹ������޹�˾, All Rights Reserved.
///
/// ��������������ڻ�ȡӲ����Ϣ
///
/// �汾        Ver 1.0
/// ����		��̫��      ʱ��		2009-02-10
/// ��ע��������ʹ���������ص���Դ
/// </summary>
/*��ȡӲ���������кŵķ�ʽ
 * ��س����������ϣ�����������
 * ��̫�� 2008.10.30
 * ʹ�þ�����
 * 
 * static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("WMI ��ʽ Ӳ�����к� : " + WMI_HD.GetHdId());

                //API ��ʽ
                Console.WriteLine("API ��ʽ Ӳ������Ϣ : ");
                HardDiskInfo hdd = AtapiDevice.GetHddInfo(0); // ��һ��Ӳ��
                Console.WriteLine("Module Number: {0}", hdd.ModuleNumber);
                Console.WriteLine("Serial Number: {0}", hdd.SerialNumber);
                Console.WriteLine("Firmware: {0}", hdd.Firmware);
                Console.WriteLine("Capacity: {0} M", hdd.Capacity);
                //
                Console.WriteLine("WMI ��ʽ �������к� : " + WMI_USB.GetUSBId());
                Console.WriteLine("OK");
                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
           
        }
 * 
 * 
 * 
 */
using System;
using System.Management;

namespace PWMIS.EnterpriseFramework.Common.Hardware
{
    /// <summary>
    /// ʹ�ãףͣɷ�ʽ��ȡӲ����Ϣ
    /// </summary>
    public class WMI_HD
    {
       /// <summary>
       /// ��ȡӲ��Ψһ���кţ����Ǿ��ţ���������Ҫ�Թ���Ա������г���
       /// </summary>
       /// <returns></returns>
        public static string GetHdId()
        {
            ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher();
            /*
             * PNPDeviceID   �����������Ĳ�����ɵģ�   
  1���ӿڣ�ͨ����   IDE��ATA��SCSI��   
  2���ͺ�   
  3�������ܣ������汾��   
  4�������ܣ�Ӳ�̵ĳ������к�   
             * 
             * 
             */


            //signature ��Ҫ�����Թ���Ա������У��������ԣ�2003ϵͳ�Ϸǹ���Ա���Ҳ�������У����������˵��������2000ϵͳ�ϻ�ȡ��ֵΪ�գ�

            wmiSearcher.Query = new SelectQuery(
            "Win32_DiskDrive",
            "",
            new string[] { "PNPDeviceID", "signature" }
            );
            ManagementObjectCollection myCollection = wmiSearcher.Get();
            ManagementObjectCollection.ManagementObjectEnumerator em =
            myCollection.GetEnumerator();
            em.MoveNext();
            ManagementBaseObject mo = em.Current;
            //string id = mo.Properties["PNPDeviceID"].Value.ToString().Trim();
            string id="";
            try
            {
                //����ʹ��signature���ӣãӣ�Ӳ�̿���û�и�����
                if (mo.Properties["signature"] != null && mo.Properties["signature"].Value != null)
                    id = mo.Properties["signature"].Value.ToString();
                else if (mo.Properties["PNPDeviceID"] != null && mo.Properties["PNPDeviceID"].Value != null)//��ֹ����
                    id = mo.Properties["PNPDeviceID"].Value.ToString();
            }
            catch
            {
                if (mo.Properties["PNPDeviceID"] != null && mo.Properties["PNPDeviceID"].Value != null)//��ֹ����
                    id = mo.Properties["PNPDeviceID"].Value.ToString();
            }
            
            return id;
        }
    }

    public class HardDiskSN
    {
        static string _serialNumber=string.Empty;
        /// <summary>
        /// ��ȡӲ�̺�
        /// </summary>
        public static string SerialNumber
        {
            get {
                if (_serialNumber == string.Empty)
                {
                    try
                    {
                        HardDiskInfo hdd = AtapiDevice.GetHddInfo(0); // ��һ��Ӳ��
                        _serialNumber ="API"+ hdd.SerialNumber;
                    }
                    catch
                    {
                        try
                        {
                            _serialNumber ="WMI"+ WMI_HD.GetHdId();
                        }
                        catch
                        {
                            _serialNumber = "FAIL111111";
                        }
                       
                    }
                   
                }
                return _serialNumber;
            }
        }
    }
}
