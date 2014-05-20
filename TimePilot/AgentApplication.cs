using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TimePilot
{
    public class AgentApplication : Android.App.Application
    {
        /*
        public override void OnCreate()
        {
            base.OnCreate();
            
        }
        
        private List<Activity> activities = new List<Activity>();  
  
        public void addActivity(Activity activity) {
            activities.Add(activity);  
        }

        public override void OnTerminate()
        {
            base.OnTerminate();


            foreach (var activity in activities)
            {
                activity.Finish();
            }

            System.Environment.Exit(0); 
        }
       */

     
            //���ڴ��ÿ��Activity��List  
            static AgentApplication instance=null;    //SysApplicationʵ��     
            List<Activity> mList = new List<Activity>();
           private AgentApplication() {  
               
               //˽�й���������ֹ����ʵ�����ö���  
            }     
     
          public  static AgentApplication getInstance() {   //ͨ��һ�������������ṩʵ��  
             if (instance == null) {     
            instance = new AgentApplication();     
        }     
        return instance;     
    }     
     
    // add Activity      
    public void addActivity(Activity activity) 
    {     
        mList.Add(activity);     
    }     
     
    public void exit() 
    {    //����List���˳�ÿһ��Activity  
         base.OnTerminate();
        try {     
            foreach (Activity activity in mList)
            {     
                if (activity != null)     
                    activity.Finish();     
            }     
        } catch (Exception e) 
        {     
                
        } 
        finally {     
            System.Environment.Exit(0);     
        }     
    }     
     
    public void onLowMemory() {     
        base.OnLowMemory();         
        System.GC.Collect();   //����ϵͳ����  
    }     
        
    }  
    
}