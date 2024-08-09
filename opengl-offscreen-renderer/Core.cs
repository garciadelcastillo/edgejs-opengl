

using System.Drawing;

namespace OpenGLOffScreenRendering
{
    public class Core
    {
        /// <summary>
        /// Ideally the function we want
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> RenderTriangle(dynamic input)
        {
            return await Task.Run(() =>
            {
                Console.WriteLine("Starting to render triangle task");

                // Parse inputs
                int screenW = input.screenW;
                int screenH = input.screenH;
                Color clr = Color.FromArgb(input.clr.r, input.clr.g, input.clr.b);
                Color bgClr = Color.FromArgb(input.bgClr.r, input.bgClr.g, input.bgClr.b);

                var pixels = Renderer.Triangle(screenW, screenH, clr, bgClr);

                return new
                {
                    pixels
                };
            });
        }

        /// <summary>
        /// Version of the above running sync?
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> RenderTriangleSync(dynamic input)
        {
            Console.WriteLine("Starting to render triangle task");

            // Parse inputs
            int screenW = input.screenW;
            int screenH = input.screenH;
            Color clr = Color.FromArgb(input.clr.r, input.clr.g, input.clr.b);
            Color bgClr = Color.FromArgb(input.bgClr.r, input.bgClr.g, input.bgClr.b);

            var pixels = Renderer.Triangle(screenW, screenH, clr, bgClr);

            return new
            {
                pixels
            };
        }

        /// <summary>
        /// Quick attempt to run the triangle rendering on the main thread, doesn't work 😭
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> RenderTriangleSyncMainThread(dynamic input)
        {
            Console.WriteLine("Starting to render triangle task");

            // Parse inputs
            int screenW = input.screenW;
            int screenH = input.screenH;
            Color clr = Color.FromArgb(input.clr.r, input.clr.g, input.clr.b);
            Color bgClr = Color.FromArgb(input.bgClr.r, input.bgClr.g, input.bgClr.b);

            byte[] pixels = new byte[0];
            Task t = new Task(() =>
            {
                pixels = Renderer.Triangle(screenW, screenH, clr, bgClr);
            });
            t.Start(TaskScheduler.FromCurrentSynchronizationContext());

            await t;

            return new
            {
                pixels
            };
        }

        /// <summary>
        /// A quick test to see if we are running in the main thread or what?
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> ThreadTest(dynamic input)
        {
            Console.WriteLine(input);
            Console.WriteLine("Starting ThreadTest... ");

            Console.WriteLine(SynchronizationContext.Current == null);

            // https://stackoverflow.com/a/36523231/1934487
            TaskScheduler syncContextScheduler;
            if (SynchronizationContext.Current != null)
            {
                syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            else
            {
                // If there is no SyncContext for this thread (e.g. we are in a unit test
                // or console scenario instead of running in an app), then just use the
                // default scheduler because there is no UI thread to sync with.
                syncContextScheduler = TaskScheduler.Current;
            }
            Console.WriteLine(syncContextScheduler);



            //return Task.Run(() =>
            //{
            //    Console.WriteLine("Checking current thread...");

            //    bool main = false;
            //    if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA ||
            //        Thread.CurrentThread.ManagedThreadId != 1 ||
            //        Thread.CurrentThread.IsBackground || 
            //        Thread.CurrentThread.IsThreadPoolThread)
            //    {
            //        Console.WriteLine("NOT in main thread");
            //        Console.WriteLine(Thread.CurrentThread.GetApartmentState());
            //        Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            //        Console.WriteLine(Thread.CurrentThread.IsBackground);
            //        Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread);
            //    } else
            //    {
            //        Console.WriteLine("YES in main thread");
            //        main = true;
            //    }

            //    return main;
            //});

            bool main = true;

            Task t = new Task(() =>
            {
                Console.WriteLine("Checking current thread...");

                if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA ||
                    Thread.CurrentThread.ManagedThreadId != 1 ||
                    Thread.CurrentThread.IsBackground ||
                    Thread.CurrentThread.IsThreadPoolThread)
                {
                    Console.WriteLine("NOT in main thread");
                    Console.WriteLine(Thread.CurrentThread.GetApartmentState());
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine(Thread.CurrentThread.IsBackground);
                    Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread);
                    main = false;
                }
                else
                {
                    Console.WriteLine("YES in main thread");
                }
            });

            //t.Start(TaskScheduler.FromCurrentSynchronizationContext());
            t.Start(syncContextScheduler);

            await t;

            Console.WriteLine("Past the await t");

            //return await Task.Run(() => true);
            return main;
        }
    }
}
