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

namespace Com.Shuyu.Gsyvideoplayer.Render.View
{
    public partial class GSYVideoGLView : global::Android.Opengl.GLSurfaceView, global::Com.Shuyu.Gsyvideoplayer.Render.View.IGSYRenderView, global::Com.Shuyu.Gsyvideoplayer.Render.View.Listener.IGLSurfaceListener, global::Com.Shuyu.Gsyvideoplayer.Utils.MeasureHelper.IMeasureFormVideoParamsListener
    {
        public void SetRenderMode(int p0)
        {
            this.RenderMode = (Android.Opengl.Rendermode)p0;
        }
    }
}