package sunmi.launcher;

import java.io.File;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;

import sunmi.launcher.utils.Adaptation;
import sunmi.launcher.utils.BitmapUtils;
import sunmi.launcher.utils.DeviceUtitls;
import sunmi.launcher.utils.FileUtils;
import sunmi.launcher.utils.LogUtil;
import sunmi.launcher.utils.ProcessUtils;
import sunmi.launcher.utils.UninstallUtils;
import android.annotation.SuppressLint;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ActivityInfo;
import android.graphics.Bitmap;
import android.graphics.Bitmap.CompressFormat;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.WindowManager;
import android.widget.Toast;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

/**
 * 
 * TODO主页面
 * 
 * @author xuron
 * @versionCode 1 <1>
 */
@SuppressLint("NewApi")
public class LauncherActivity extends UnityPlayerActivity {
//public class LauncherActivity extends Activity {

	private static final String TAG = "LauncherActivity";
	private static final String ICON_FOLDER = FileUtils.getSDCardPath()+"/launcher_icon/";
	
	
	/**
	 * 应用数据的Controller
	 */
	private AppController appController;
	
	private Gson mGson;
	
	private BitmapUtils mBitmapUtils;
	
	private Type appListType;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		registerBoradcastReceiver();
//		setContentView(R.layout.layout_home);
		init();
//		loadApp();
	}


	/**
	 * 初始化
	 */
	private void init() {
		Adaptation.init(this);
		initLauncherModel();
		
		appController = AppController.getInstance(this);
		mGson = new Gson();
		appListType = new TypeToken<List<AppModel>>(){}.getType();
		mBitmapUtils = new BitmapUtils(this);
		FileUtils.createFolder(ICON_FOLDER);
		// 开启监听推送通知服务
		// Intent intent = new
		// Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
		// startActivity(intent);
	}


	private void initLauncherModel() {
		String model = DeviceUtitls.getDeviceModel();
		LauncherModel launcherModel = LauncherModel.getInstance();
		launcherModel.model = model;
		launcherModel.statusBarHeight = getStatusBarHeight();
		launcherModel.screenWidth = Adaptation.screenWidth;
		launcherModel.screenHeight = Adaptation.screenHeight - launcherModel.statusBarHeight;
		launcherModel.proportion = Adaptation.proportion;
		switch (Adaptation.proportion) {
		case Adaptation.SCREEN_9_16:
			launcherModel.pageItemNum = 6;
			launcherModel.rowNum = 3;
			launcherModel.columnNum = 2;
			launcherModel.iconScaleOnScreenW = 0.35f;
			launcherModel.fontSize = 30;
			break;
		case Adaptation.SCREEN_3_4:
			launcherModel.pageItemNum = 6;
			launcherModel.rowNum = 3;
			launcherModel.columnNum = 2;
			launcherModel.iconScaleOnScreenW = 0.3f;
			launcherModel.fontSize = 27;
			break;
		case Adaptation.SCREEN_4_3:
			launcherModel.pageItemNum = 6;
			launcherModel.rowNum = 2;
			launcherModel.columnNum = 3;
			launcherModel.iconScaleOnScreenW = 0.33f;
			launcherModel.fontSize = 27;
			break;
		case Adaptation.SCREEN_16_9:
			launcherModel.pageItemNum = 8;
			launcherModel.rowNum = 2;
			launcherModel.columnNum = 4;
			launcherModel.iconScaleOnScreenW = 0.3f;
			launcherModel.fontSize = 35;
			break;

		default:
			break;
		}
		launcherModel.itemWidth = launcherModel.screenWidth / launcherModel.columnNum;
		launcherModel.itemHeight = launcherModel.screenHeight / launcherModel.rowNum;
		launcherModel.iconWidth = (int) (launcherModel.itemWidth*launcherModel.iconScaleOnScreenW);
		
	}
	
	public int getStatusBarHeight() {
		  int result = 0;
		  int resourceId = getResources().getIdentifier("status_bar_height", "dimen", "android");
		  if (resourceId > 0) {
		      result = getResources().getDimensionPixelSize(resourceId);
		  }
		  return result;
	}

	/*---------Unity Message--------*/
	public void openApp(String appJson){
		AppModel model = mGson.fromJson(appJson, AppModel.class);
		LogUtil.d(TAG, "openApp app:"+model.getPackageName());
		appController.openApp(model);
	}
	
	public void askForUninstall(String gameObj, String callback, String packageName){
		if(appController.isSystemAppByPkName(packageName)){
			runOnUiThread(new Runnable() {
				
				@Override
				public void run() {
					Toast.makeText(getBaseContext(), "为了保证完整的系统功能，无法卸载系统内建的应用程序", 1).show();
					
				}
			});
		}else{
			boolean b = UninstallUtils.canSilenceUninstall(this, packageName);
			UnityPlayer.UnitySendMessage(gameObj, callback, b+"");
		}
	}
	
	public void silenceUninstall(String modelJson){
		AppModel model = mGson.fromJson(modelJson, AppModel.class);
		LogUtil.d(TAG, "silenceUninstall app:"+model.getPackageName());
		UninstallUtils.silenceUninstall(this, model);
	}
	
	public void uninstall(final String json){
		runOnUiThread(new Runnable() {
			
			@Override
			public void run() {
				AppModel model = mGson.fromJson(json, AppModel.class);
				LogUtil.d(TAG, "uninstall app:"+model.getPackageName());
				UninstallUtils.uninstall(LauncherActivity.this, model);
			}
		});

	}
	
	public void saveAppList(String appListJson){
		LogUtil.d(TAG, "saveAppList");
		List<AppModel> appList = mGson.fromJson(appListJson, appListType);
		appController.appListChange(appList);
		LogUtil.d(TAG, "saveAppList size:"+ appList.size());
//		LogUtil.d(TAG, "appListJson:"+ appListJson);
		appController.save2Cache();
	}
	
	public void debug(String tag, String msg){
		LogUtil.d(tag, msg);
	}
	
	public void getLauncherModel(String gameObj, String callback){
		String json = mGson.toJson(LauncherModel.getInstance());
		Log.d(TAG, json);
		UnityPlayer.UnitySendMessage(gameObj, callback, json);
	}
	
	public void showStatusBar(){
		runOnUiThread(new Runnable() {

			@Override
			public void run() {
				getWindow().clearFlags(
						WindowManager.LayoutParams.FLAG_FULLSCREEN);
				//透明状态栏  
				getWindow().addFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS);  
				//透明导航栏  
				getWindow().addFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_NAVIGATION);  

			}
		});
	}

	public void getAppList(final String gameObj, final String callback) {
				FileUtils.deleteFile(ICON_FOLDER);
				final List<AppModel> list = loadApp();
						
				String json = mGson.toJson(list);
				Log.d(TAG, json);
				Log.d(TAG, "UnitySendMessage gameObj:"+gameObj+",callback:"+callback);
				UnityPlayer.UnitySendMessage(gameObj, callback, json);
	}


	private List<AppModel> loadApp() {
		final List<AppModel> list = appController
				.getCacheList(LauncherActivity.this);
		Random random =new Random();
//		Log.d(TAG, "list"+list.toString());
		for (AppModel app : list) {
			if(app.isPlaceholder())continue;
			_loadApp(app, random.nextInt(100));
		}
		return list;
	}
	
	public void _loadApp(AppModel app, int random){
		Drawable icon = appController.getIcon(app);
		if (icon == null){
			Log.d(TAG, "icon == null:"+ app.getPackageName());
			return;
		}
			
		BitmapDrawable drawable = null;
		if(icon instanceof BitmapDrawable){
			drawable = (BitmapDrawable) icon;
		}else {
			Log.d(TAG, "not BitmapDrawable:" + app.packageName);
			icon = appController.getIconFromPackageName(app.packageName);
		}
		
		drawable = (BitmapDrawable) icon;
		
		Bitmap bitmap = drawable.getBitmap();
		
		File file = mBitmapUtils.saveBitmap(bitmap, ICON_FOLDER, app.packageName+random,
				CompressFormat.PNG);
		app.setIconUrl(file.getAbsolutePath());
//		Log.d(TAG, "bitmap path:" + file.getAbsolutePath());
	}
	
	/*--------------------------------------------------*/

	@Override
	protected void onNewIntent(Intent intent) {
		super.onNewIntent(intent);
	}

	/**
	 * 监听返回按钮，如果处于编辑模式则取消编辑，如果不在第一屏则返回上一屏
	 */
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (KeyEvent.KEYCODE_BACK == keyCode) {
			return true;
		}
		return super.onKeyDown(keyCode, event);
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();
		unregisterReceiver(newAppReceiver);
		unregisterReceiver(updateAppReceiver);
		unregisterReceiver(removeAppReceiver);
		unregisterReceiver(refreshReceiver);
//		unregisterReceiver(pushNumReceiver);
		unregisterReceiver(installerReceiver);
		unregisterReceiver(removeTaskReceiver);
		unregisterReceiver(resumeStopAppReceiver);
	}

	@Override
	protected void onResume() {
		super.onResume();
		LogUtil.d(TAG, "onResume");
		// 如果屏幕是不是竖屏，则改为竖屏
		if (getRequestedOrientation() != ActivityInfo.SCREEN_ORIENTATION_PORTRAIT) {
			setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
		}

		notifyOpenApp();
	}


	private void notifyOpenApp() {
		final AppModel openAppModel = appController.getOpenAppModel();
		if (openAppModel != null) {
			new Handler().postDelayed(new Runnable() {

				@Override
				public void run() {
					// 刷新Layout
					if (openAppModel.getPushNum() > 0) {
						openAppModel.setPushNum(0);
						Intent refreshIntent = new Intent();
						refreshIntent.setAction(BroadcastConstants.APP_REFRESH);
						refreshIntent.putExtra("AppModel", openAppModel);
						sendBroadcast(refreshIntent);
					}
				}
			}, 2000);

		}
	}

	@Override
	protected void onPause() {
		super.onPause();
		LogUtil.d(TAG, "onPause");
		if (appController != null) {
			appController.save2Cache();
		}
	}

	@Override
	protected void onStop() {
		super.onStop();
	}

	/**
	 * 注册广播
	 */
	public void registerBoradcastReceiver() {

		// 新增app广播
		IntentFilter newAppFilter = new IntentFilter();
		newAppFilter.addAction(BroadcastConstants.NEWAPP);
		registerReceiver(newAppReceiver, newAppFilter);
		// 更新App广播
		IntentFilter updateAppFilter = new IntentFilter();
		updateAppFilter.addAction(BroadcastConstants.APP_UPDATE);
		registerReceiver(updateAppReceiver, updateAppFilter);
		// 卸载应用广播
		IntentFilter removeFilter = new IntentFilter();
		removeFilter.addAction(BroadcastConstants.APP_REMOVE);
		registerReceiver(removeAppReceiver, removeFilter);
		// 注册点击app事件刷新layout广播
		IntentFilter refreshFilter = new IntentFilter();
		refreshFilter.addAction(BroadcastConstants.APP_REFRESH);
		registerReceiver(refreshReceiver, refreshFilter);
		// 注册刷新推送消息刷新layout广播
//		IntentFilter pushNumFilter = new IntentFilter();
//		pushNumFilter.addAction(Constants.HANDLE_PUSHNUM_RECEIVER);
//		registerReceiver(pushNumReceiver, pushNumFilter);
		// 注册安装器回调广播
		IntentFilter installerFilter = new IntentFilter();
		installerFilter.addAction(BroadcastConstants.APP_INSTALL);
		registerReceiver(installerReceiver, installerFilter);
		// 移除任务广播
		IntentFilter removeTaskFilter = new IntentFilter();
		removeTaskFilter.addAction(BroadcastConstants.INSTALL_TASK_REMOVE);
		registerReceiver(removeTaskReceiver, removeTaskFilter);
		// 恢复停用应用广播
		IntentFilter resumeStopAppFilter = new IntentFilter();
		resumeStopAppFilter.addAction(BroadcastConstants.RESUMESTOPAPP);
		registerReceiver(resumeStopAppReceiver, resumeStopAppFilter);
	}


	/**
	 * 新增app广播接收者
	 * 
	 * @author 荣
	 * 
	 */
	public BroadcastReceiver newAppReceiver = new BroadcastReceiver() {

		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(BroadcastConstants.NEWAPP)) {
				String packageName = intent.getStringExtra("packageName");
				if (!TextUtils.isEmpty(packageName)) {
					//判断是否可以加入缓存列表
					if(!appController.addable(packageName)){
						return;
					}
					
					List<AppModel> list = appController.getModelListByPackageName(packageName);
					if(list==null||list.size()==0)return;
					
					Random random =new Random();
					for(AppModel appModel : list){
						appModel.setBeUsed(false);
						appModel.setAppStatus(0);
					    appController.addNew(appModel);
					    _loadApp(appModel, random.nextInt(100));
					}
					String json = mGson.toJson(list);
					UnityPlayer.UnitySendMessage("LauncherController", "MessageInstalled", json);
					LogUtil.d(TAG, "newAppReceiver json:"+json);
				}
			}
		}
	};

	/**
	 * 恢复停用应用的广播
	 */
	public BroadcastReceiver resumeStopAppReceiver = new BroadcastReceiver() {

		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(BroadcastConstants.RESUMESTOPAPP)) {
				String packageName = intent.getStringExtra("packageName");
				if (!TextUtils.isEmpty(packageName)) {
					boolean displayable = appController.displayable(packageName);
					if(displayable){
						AppModel appModel = new AppModel();
						appModel.setAppStatus(0);
						appModel.setBeUsed(false);
						String appName = appController.getAppNameByPackage(packageName);
						appModel.setAppName("" + appName);
						appModel.setPackageName("" + packageName);
						appController.addOnFirstByCurrentPage(appModel);
					}
					

				}
			}
		}
	};
	/**
	 * 应用更新app
	 */
	public BroadcastReceiver updateAppReceiver = new BroadcastReceiver() {

		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(BroadcastConstants.APP_UPDATE)) {
				String packageName = intent.getStringExtra("packageName");
				if (!TextUtils.isEmpty(packageName)) {
					//判断缓存列表是否存在此app
					boolean contains = appController.containsByName(packageName);
					if(!contains)return;
					//判断是否可添加到缓存列表
					boolean displayable = appController.addable(packageName);
					if(!displayable)return;
					
					AppModel appModel = new AppModel();
					appModel.setAppStatus(0);
					appModel.setBeUsed(false);
					String appName = appController.getAppNameByPackage(packageName);
					appModel.setAppName("" + appName);
					appModel.setPackageName("" + packageName);
					appController.update(appModel);
					int indexOf = appController.indexOf(appModel);
				}
			}
		}
	};


	/**
	 * 删除app广播
	 */
	public BroadcastReceiver removeAppReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(BroadcastConstants.APP_REMOVE)) {
				String packageName = intent.getStringExtra("packageName");
				if (!TextUtils.isEmpty(packageName)) {
					AppModel appModel = appController.getByPackageName(packageName);
					if(appModel.isPlaceholder()){return;}
					// 是否包含这个app
					if (!appController.contains(appModel)) {
						return;
					}
					final int delPos = appController.indexOf(appModel);
					String appJson = mGson.toJson(appModel);
					UnityPlayer.UnitySendMessage("LauncherController", "MessageUninstalled", appJson);
					LogUtil.d(TAG, "removeAppReceiver appJson:"+appJson);
				}
			}
		}
	};
	/**
	 * 监听点击app事件刷新layout
	 */
	public BroadcastReceiver refreshReceiver = new BroadcastReceiver() {

		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(BroadcastConstants.APP_REFRESH)) {
				AppModel appModel = (AppModel) intent.getSerializableExtra("AppModel");
				appController.update(appModel);
				int position = appController.indexOf(appModel);
				if (position != -1) {
				}
			}
		}
	};

	/**
	 * 根据推送消息的数量刷新
	 */
//	public BroadcastReceiver pushNumReceiver = new BroadcastReceiver() {
//
//		@Override
//		public void onReceive(Context context, Intent intent) {
//			if (intent.getAction().equals(Constants.HANDLE_PUSHNUM_RECEIVER)) {
//				String packageName = (String) intent.getSerializableExtra("PackageName");
//				boolean isExist = appController.isExistAppListByName(packageName);
//				if (!isExist) {
//					return;
//				}
//				AppModel appModel = new AppModel();
//				String appName = appController.getAppNameByPackage(getBaseContext(), packageName);
//				appModel.setAppName(appName);
//				appModel.setPackageName(packageName);
//				appModel.setPushNum(1);
//				appModels = appController.updateAppModel(appModel);
//				mAdapter.notifyDataSetChanged();
//			}
//		}
//	};

	/**
	 * 接收安装器回调的状态
	 */
	public BroadcastReceiver installerReceiver = new BroadcastReceiver() {

		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(BroadcastConstants.APP_INSTALL)) {
				ArrayList<AppModel> appModelList = (ArrayList<AppModel>) intent
						.getSerializableExtra("list");
				AppModel appModel = (AppModel) intent.getSerializableExtra("AppModel");
				if (appModelList != null && appModelList.size() > 0) {
					appController.addList(appModelList);
				} else if (appModel != null) {
					if (!appController.getAppModelList().contains(appModel)) {
						appController.update(appModel);
					} else {
						appController.update(appModel);
						int position = appController.indexOf(appModel);
					}
				}
			}
		}
	};

	/**
	 * 移除任务广播
	 */
	public BroadcastReceiver removeTaskReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			if (intent.getAction().equals(BroadcastConstants.INSTALL_TASK_REMOVE)) {
				final AppModel appModel = (AppModel) intent.getSerializableExtra("AppModel");
				if (appModel != null) {
					if (ProcessUtils.isInstalled(context, appModel.getPackageName())) {
						appModel.setAppStatus(0);
						appController.update(appModel);
					} else {
						appController.remove(appModel);
					}
				}
			}
		}
	};
	



}
