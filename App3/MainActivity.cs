
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Json;
using Android.Runtime;
using Android.Views;




namespace App3
{
     


    [Activity( Label = "Lista Zakupów", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        LinearLayout singleTaskLayout;
        LinearLayout.LayoutParams  singleTaskTextParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
        LinearLayout.LayoutParams controlButtonParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
        LinearLayout.LayoutParams p = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
        LinearLayout root;
        LinearLayout taskButtonArea;




        ProgressBar spinner;
        public int DpToPixels(int dp)
        {
            return dp * (int)Resources.DisplayMetrics.Density;
        }


        class SingleTask
        {
            public int id;
            public string value;
            public int state;
            public string added;
        
        };


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

           

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            spinner = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            
                spinner.Visibility = Android.Views.ViewStates.Gone;

            root = FindViewById<LinearLayout>(Resource.Id.linearLayout3);

            p.SetMargins(10, 10, 10, 10);
            p.Height = DpToPixels(70);

           
            ScrollView sc = FindViewById<ScrollView>(Resource.Id.scrollView1);
           // sc.ScrollChange += ScrollView1_ScrollChange;

            Button button1 = FindViewById<Button>(Resource.Id.button1);


            // When the user clicks the button ...
            button1.Click +=  (sender, e) => {
               RefreshTasks();
            };

            RefreshTasks();


        }

        private async void RefreshTasks()
        {
            root.RemoveAllViews();
            spinner.Visibility = Android.Views.ViewStates.Visible;
            root.AddView(spinner);
            JsonValue json = await GetTasks();

            ParseAndDisplayTasks(json);
            spinner.Visibility = Android.Views.ViewStates.Gone;
        }
      

        private void ScrollView1_ScrollChange(object sender, EventArgs e)
        {
            ScrollView scrollView = sender as ScrollView;

            double scrollingSpace = scrollView.GetChildAt(0).Height - scrollView.Height;

            if (scrollView.ScrollY<=0) // Touched bottom
            {
                // Do the things you want to do
                Toast.MakeText(this, "You have reached to the top!", ToastLength.Short).Show();
            }
        }

        private async Task<JsonValue> GetTasks()
        {
           
            string url = "https://lista-zadan.herokuapp.com/api/tasks";
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            try
            { 
            using (WebResponse response = await request.GetResponseAsync())
            {
                    
                    // Get a stream representation of the HTTP web response:
                    using (System.IO.Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                       // Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                        // Return the JSON document:
                        return jsonDoc;
                    }

                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Problem z połączeniem do API (przez internet)...", ToastLength.Short).Show();
                return null;
            };



        }

        private void ParseAndDisplayTasks(JsonValue json)
        {
            if(json==null)
                          
                return;
           
            // Extract the array of name/value results for the field name "weatherObservation". 
            JsonValue data = json["data"];
            SingleTask currentItem = new SingleTask();
            foreach (JsonValue singleItem in data)
            {
                
               
                currentItem.id =(int)singleItem["id"];
                currentItem.value = (string)singleItem["value"];
                DisplayNewTask(currentItem);


            }
             
        }
        private void DisplayNewTask(SingleTask t)
        {
            singleTaskLayout = new LinearLayout(this);
            singleTaskLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#F7CE5B"));
            singleTaskLayout.Orientation = Orientation.Horizontal;
            singleTaskLayout.SetPadding(0,0,0,0);

            //text value
            TextView value = new TextView(this);
            value.SetTextColor(Android.Graphics.Color.ParseColor("#444444"));
            value.SetTextSize(Android.Util.ComplexUnitType.Sp,20);
            LinearLayout textLayout=new LinearLayout(this);
            singleTaskTextParams.TopMargin = DpToPixels(20);
            singleTaskTextParams.LeftMargin = DpToPixels(5);
            value.LayoutParameters = singleTaskTextParams;
            value.Text = t.value;

            singleTaskLayout.AddView(value);

            //control buttons

            taskButtonArea = new LinearLayout(this);
            LinearLayout.LayoutParams test = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
            test.Gravity = Android.Views.GravityFlags.Right;
            test.Weight = 1;
            taskButtonArea.LayoutParameters = test;
            taskButtonArea.SetGravity(Android.Views.GravityFlags.Right);
           
            Button btn_delete = new Button(this);
          
            btn_delete.Text = "X";
            btn_delete.SetPadding(2, 0, 2, 0);
            btn_delete.SetBackgroundColor(Android.Graphics.Color.ParseColor("#93032E"));
            //btn_delete.Gravity = Android.Views.GravityFlags.Right;
            //  controlButtonParams.Width = LinearLayout.LayoutParams.WrapContent;
            // btn_delete.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent));
            btn_delete.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
            // btn_delete.RefreshDrawableState();
          //  btn_delete.Gravity = Android.Views.GravityFlags.Right;



          taskButtonArea.AddView(btn_delete);
            singleTaskLayout.AddView(taskButtonArea);



            root.AddView(singleTaskLayout,p);
        }
    }
}

