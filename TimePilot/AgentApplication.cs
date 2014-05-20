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

     
            //用于存放每个Activity的List  
            static AgentApplication instance=null;    //SysApplication实例     
            List<Activity> mList = new List<Activity>();
           private AgentApplication() {  
               
               //私有构造器，防止外面实例化该对象，  
            }     
     
          public  static AgentApplication getInstance() {   //通过一个方法给外面提供实例  
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
    {    //遍历List，退出每一个Activity  
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
        System.GC.Collect();   //告诉系统回收  
    }     
        
    }  
    
}