using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Shuyu.Gsyvideoplayer.Builder;
using Com.Shuyu.Gsyvideoplayer.Listener;
using Com.Shuyu.Gsyvideoplayer.Utils;
using Com.Shuyu.Gsyvideoplayer.Video.Base;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace ICE_Binding
{
    [Android.App.Activity(Label = "ScrollingActivity", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class ScrollingActivity : AppCompatActivity, View.IOnClickListener, ILockClickListener, IGSYVideoProgressListener
    {

        private static bool isPlay;
        private static bool isPause;
        private static bool isSamll;

        private static OrientationUtils orientationUtils;
        private LandLayoutVideo detailPlayer;
        private static AppBarLayout appBar;
        private static FloatingActionButton fab;
        private static CoordinatorLayout root;
        private static Context context;

        private IAppBarStateChangeListener appBarStateChangeListener;

        private CollapsingToolbarLayout toolBarLayout;

        private static AppBarStateChangeListener.State curState;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.activity_scrolling);

                appBarStateChangeListener = new IAppBarStateChangeListener();
                context = this;// this.BaseContext;
                initView();

                string url = "http://9890.vod.myqcloud.com/9890_4e292f9a3dd011e6b4078980237cc3d3.f20.mp4";

                //增加封面
                ImageView imageView = new ImageView(this);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);

                ResolveNormalVideoUI();

                //外部辅助的旋转，帮助全屏
                orientationUtils = new OrientationUtils(this, detailPlayer);
                //初始化不打开外部的旋转
                orientationUtils.Enable = false;//.setEnable(false);

                GSYVideoOptionBuilder gsyVideoOption = new GSYVideoOptionBuilder();
                gsyVideoOption.SetThumbImageView(imageView)
                  .SetIsTouchWiget(true)
                  .SetRotateViewAuto(false)
                  .SetLockLand(false)
                  .SetShowFullAnimation(false)
                  .SetNeedLockFull(true)
                  .SetSeekRatio(1)
                  .SetUrl(url)
                  .SetCacheWithPlay(false)
                  .SetVideoTitle("测试视频")
                  .SetVideoAllCallBack(new IGSYSampleCallBack())
                  .SetLockClickListener(this)
                  .SetGSYVideoProgressListener(this)
                  .Build(detailPlayer);

                detailPlayer.FullscreenButton.Click += (sender, e) =>
                {
                    //直接横屏
                    orientationUtils.ResolveByClick();

                    //第一个true是否需要隐藏actionbar，第二个true是否需要隐藏statusbar
                    detailPlayer.StartWindowFullscreen(this, true, true);
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error("", ex.ToString());
            }

            //detailPlayer..setLinkScroll(true);
        }


        private void initView()
        {
            Toolbar toolbar = (Toolbar)FindViewById(Resource.Id.toolbar);
            detailPlayer = FindViewById<LandLayoutVideo>(Resource.Id.detail_player);
            root = (CoordinatorLayout)FindViewById(Resource.Id.root_layout);

            SetSupportActionBar(toolbar);
            toolBarLayout = (CollapsingToolbarLayout)FindViewById(Resource.Id.toolbar_layout);
            toolBarLayout.Title = "";//.setTitle(getTitle());

            fab = (FloatingActionButton)FindViewById(Resource.Id.fab);
            fab.SetOnClickListener(this);
            appBar = (AppBarLayout)FindViewById(Resource.Id.app_bar);
            appBar.AddOnOffsetChangedListener(appBarStateChangeListener);
        }


        public void OnClick(View v)
        {
            detailPlayer.StartPlayLogic();
            root.RemoveView(fab);
        }

        public override void OnBackPressed()
        {

            if (orientationUtils != null)
            {
                orientationUtils.BackToProtVideo();
            }
            //detailPlayer.GSYVideoManager
            //if (GSYVideoManager.backFromWindowFull(this))
            //{
            //    return;
            //}

            base.OnBackPressed();
        }



        private void ResolveNormalVideoUI()
        {
            //增加title
            detailPlayer.TitleTextView.Visibility = ViewStates.Gone;//.getTitleTextView().setVisibility(View.GONE);
            detailPlayer.BackButton.Visibility = ViewStates.Gone;//.getBackButton().setVisibility(View.GONE);
        }

        private GSYVideoPlayer GetCurPlay()
        {
            if (detailPlayer.FullWindowPlayer != null)
            {
                return detailPlayer.FullWindowPlayer;// getFullWindowPlayer();
            }
            return detailPlayer;
        }

        protected override void OnPause()
        {
            GetCurPlay().OnVideoPause();
            base.OnPause();
            isPause = true;
        }

        protected override void OnResume()
        {
            GetCurPlay().OnVideoResume();
            appBar.AddOnOffsetChangedListener(appBarStateChangeListener);
            base.OnResume();
            isPause = false;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (isPlay)
            {
                GetCurPlay().Release();
            }
            if (orientationUtils != null)
                orientationUtils.ReleaseListener();
        }


        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);
                //如果旋转了就全屏
                if (isPlay && !isPause)
                {
                    detailPlayer.OnConfigurationChanged(this, newConfig, orientationUtils, true, true);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error("", ex.ToString());
            }

        }

        public void OnClick(View view, bool @lock)
        {

            if (orientationUtils != null)
            {
                //配合下方的onConfigurationChanged
                orientationUtils.Enable = !@lock;//.setEnable(!lock) ;
            }
        }

        public void OnProgress(int progress, int secProgress, int currentPosition, int duration)
        {
            Debuger.PrintfLog(" progress " + progress + " secProgress " + secProgress + " currentPosition " + currentPosition + " duration " + duration);
        }

        public class IGSYSampleCallBack : GSYSampleCallBack
        {
            public override void OnPrepared(string url, Java.Lang.Object[] objects)
            {
                Debuger.PrintfError("***** onPrepared **** " + objects[0]);
                Debuger.PrintfError("***** onPrepared **** " + objects[1]);
                base.OnPrepared(url, objects);
                //开始播放了才能旋转和全屏
                orientationUtils.Enable = true;//.setEnable(true);
                isPlay = true;
                root.RemoveView(fab);
            }

            public override void OnEnterFullscreen(string url, Java.Lang.Object[] objects)
            {
                base.OnEnterFullscreen(url, objects);
                Debuger.PrintfError("***** onEnterFullscreen **** " + objects[0]);//title
                Debuger.PrintfError("***** onEnterFullscreen **** " + objects[1]);//当前全屏player
            }

            public override void OnAutoComplete(string url, Java.Lang.Object[] objects)
            {
                base.OnAutoComplete(url, objects);
            }

            public override void OnClickStartError(string url, Java.Lang.Object[] objects)
            {
                base.OnClickStartError(url, objects);
            }

            public override void OnQuitFullscreen(string url, Java.Lang.Object[] objects)
            {
                base.OnQuitFullscreen(url, objects);
                Debuger.PrintfError("***** onQuitFullscreen **** " + objects[0]);//title
                Debuger.PrintfError("***** onQuitFullscreen **** " + objects[1]);//当前非全屏player
                if (orientationUtils != null)
                {
                    orientationUtils.BackToProtVideo();
                }
            }
        }

        public class IAppBarStateChangeListener : AppBarStateChangeListener
        {
            public override void OnStateChanged(AppBarLayout appBarLayout, State state)
            {
                if (state == AppBarStateChangeListener.State.EXPANDED)
                {
                    //展开状态
                    curState = state;
                    //toolBarLayout.setTitle("");
                }
                else if (state == AppBarStateChangeListener.State.COLLAPSED)
                {
                    //折叠状态
                    //如果是小窗口就不需要处理
                    // toolBarLayout.setTitle("Title");
                    if (!isSamll && isPlay)
                    {
                        isSamll = true;
                        int size = CommonUtil.Dip2px(context, 150);
                        //detailPlayer.ShowSmallVideo(new Point(size, size), true, true);
                        orientationUtils.Enable = false;//.etEnable(false);
                    }
                    curState = state;
                }
                else
                {
                    if (curState == AppBarStateChangeListener.State.COLLAPSED)
                    {
                        //由折叠变为中间状态
                        //toolBarLayout.setTitle("");
                        if (isSamll)
                        {
                            isSamll = false;
                            orientationUtils.Enable = false;//.setEnable(true);
                                                            //必须
                                                            //    detailPlayer.postDelayed(new Runnable() {
                                                            //    @Override
                                                            //    public void run()
                                                            //    {
                                                            //        detailPlayer.hideSmallVideo();
                                                            //    }
                                                            //}, 50);
                        }
                    }
                    curState = state;
                    //中间状态
                }
            }
        }
    }

    public abstract class AppBarStateChangeListener : AppBarLayout.IOnOffsetChangedListener
    {
        public enum State
        {
            EXPANDED,
            COLLAPSED,
            IDLE
        }

        private State mCurrentState = State.IDLE;

        public IntPtr Handle
        {
            get { return IntPtr.Zero; }
        }


        public void OnOffsetChanged(AppBarLayout appBarLayout, int i)
        {
            if (i == 0)
            {
                if (mCurrentState != State.EXPANDED)
                {
                    OnStateChanged(appBarLayout, State.EXPANDED);
                }
                mCurrentState = State.EXPANDED;
            }
            else if (Math.Abs(i) >= appBarLayout.TotalScrollRange)
            {
                if (mCurrentState != State.COLLAPSED)
                {
                    OnStateChanged(appBarLayout, State.COLLAPSED);
                }
                mCurrentState = State.COLLAPSED;
            }
            else
            {
                if (mCurrentState != State.IDLE)
                {
                    OnStateChanged(appBarLayout, State.IDLE);
                }
                mCurrentState = State.IDLE;
            }
            appBarLayout.AddOnOffsetChangedListener(this);
        }

        public void Dispose()
        {
            return;
        }

        public abstract void OnStateChanged(AppBarLayout appBarLayout, State state);
    }
}