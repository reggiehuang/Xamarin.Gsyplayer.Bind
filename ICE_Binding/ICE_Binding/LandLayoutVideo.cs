using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Shuyu.Gsyvideoplayer.Video;
using Com.Shuyu.Gsyvideoplayer.Video.Base;

namespace ICE_Binding
{
    public class LandLayoutVideo : StandardGSYVideoPlayer
    {
        private bool isLinkScroll = false;
        private GestureDetector gestureDetector;
        //private static LandLayoutVideo llVideo;

        public LandLayoutVideo(Context context, Java.Lang.Boolean fullFlag) : base(context, fullFlag)
        {

        }

        public LandLayoutVideo(Context context) : base(context)
        {

        }

        public LandLayoutVideo(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        protected LandLayoutVideo(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }
        public class GDOnGestureListener : GestureDetector.SimpleOnGestureListener
        {
            LandLayoutVideo llVideo;
            public GDOnGestureListener(LandLayoutVideo model)
            {
                llVideo = model;
            }
            public override bool OnDoubleTap(MotionEvent e)
            {
                llVideo.TouchDoubleUp();
                return base.OnDoubleTap(e);
            }

            public override bool OnSingleTapConfirmed(MotionEvent e)
            {
                if (!llVideo.MChangePosition && !llVideo.MChangeVolume && !llVideo.MBrightness)
                {
                    llVideo.OnClickUiToggle();
                }

                return base.OnSingleTapConfirmed(e);
            }


            public override void OnLongPress(MotionEvent e)
            {
                base.OnLongPress(e);

            }
        }

        protected override void Init(Context context)
        {

            try
            {

                base.Init(context);

                Post(() => { gestureDetector = new GestureDetector(Context, new GDOnGestureListener(this)); });

            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error("", ex.ToString());
            }
            //View view = LayoutInflater.From(this.Context).Inflate(GetLayoutId(), this);
            //SetContentView(Resource.Layout.activity_main);
            //base.Init(context);

            //new Handler().Post(() =>
            //{
            //    gestureDetector = new GestureDetector(Context, new GDOnGestureListener());
            //});

        }

        //这个必须配置最上面的构造才能生效

        public override int LayoutId => GetLayoutId();

        public int GetLayoutId()
        {

            if (MIfCurrentIsFullscreen)
            {
                return Resource.Layout.sample_video_land;
            }
            return Resource.Layout.sample_video_normal;
        }

        protected override void UpdateStartImage()
        {
            try
            {
                if (MIfCurrentIsFullscreen)
                {
                    if (MStartButton is ImageView)
                    {
                        ImageView imageView = (ImageView)MStartButton;
                        if (MCurrentState == CurrentStatePlaying)
                        {
                            imageView.SetImageResource(Resource.Drawable.video_click_pause_selector);
                        }
                        else if (MCurrentState == CurrentStateError)
                        {
                            imageView.SetImageResource(Resource.Drawable.video_click_play_selector);
                        }
                        else
                        {
                            imageView.SetImageResource(Resource.Drawable.video_click_play_selector);
                        }
                    }
                }
                else
                {
                    base.UpdateStartImage();
                }
            }
            catch (Exception ex)
            {
                Log.Error("", ex.ToString());
            }

        }


        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            try
            {
                if (isLinkScroll && !MIfCurrentIsFullscreen)
                {
                    Parent.RequestDisallowInterceptTouchEvent(true);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error("", ex.ToString());
            }
            return base.OnInterceptTouchEvent(ev);
        }
        protected override void ResolveNormalVideoShow(View oldF, ViewGroup vp, GSYVideoPlayer gsyVideoPlayer)
        {
            try
            {
                LandLayoutVideo landLayoutVideo = (LandLayoutVideo)gsyVideoPlayer;
                landLayoutVideo.DismissProgressDialog();
                landLayoutVideo.DismissVolumeDialog();
                landLayoutVideo.DismissBrightnessDialog();
                base.ResolveNormalVideoShow(oldF, vp, gsyVideoPlayer);
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error("", ex.ToString());
            }
        }

        public void setLinkScroll(bool linkScroll)
        {
            isLinkScroll = linkScroll;
        }
    }
}