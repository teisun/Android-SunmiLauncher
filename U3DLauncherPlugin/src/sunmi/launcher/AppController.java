package sunmi.launcher;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import android.annotation.SuppressLint;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.content.pm.ResolveInfo;
import android.content.res.Resources;
import android.graphics.drawable.Drawable;
import android.os.Build;
import android.text.TextUtils;
import android.util.DisplayMetrics;
import android.util.Log;

/**
 * 显示数据源控制器
 * 
 */
public class AppController {
	private static final String TAG = "AppController";
	/**
	 * 缓存对象
	 */
	private ACache aCache;
	/**
	 * 获取appModelList
	 */
	private ArrayList<AppModel> appModelList = new ArrayList<AppModel>();
	/**
	 * 记录当前被打开的app
	 */
	private AppModel openAppModel;

	private static AppController appController;
	private Context mContext;
	private LauncherModel launcherModel;

	private AppController(Context c) {
		mContext = c;
		launcherModel = LauncherModel.getInstance();
	}

	/**
	 * 单例模式
	 */
	public static AppController getInstance(Context c) {
		if (appController == null) {
			appController = new AppController(c);
		}
		return appController;
	}

	/**
	 * 返回当前的app列表
	 * 
	 * @return
	 */
	public List<AppModel> getAppModelList() {
		return appModelList;
	}
	
	public void appListChange(List<AppModel> list){
		appModelList.clear();
		appModelList.addAll(list);
	}

	/**
	 * 获取当前打开的app
	 * 
	 * @return
	 */
	public AppModel getOpenAppModel() {
		return openAppModel;
	}

	/**
	 * 设置当前被打开的app
	 * 
	 * @param model
	 */
	public void setOpenAppModel(AppModel model) {
		this.openAppModel = model;
		if (appModelList.contains(model)) {
			int position = appModelList.indexOf(model);
			AppModel appModel = appModelList.get(position);
			// 如果AppModel的beUsed这个状态为true则设为true，为false设为原始状态（配合消息推送）
			if (model.isBeUsed()) {
				appModel.setBeUsed(true);
			} else {
				appModel.setBeUsed(appModel.isBeUsed());
			}
			// 如果app推送消息数量大于0就累加，反之置为0
			if (model.getPushNum() > 0) {
				appModel.setPushNum(appModel.getPushNum() + model.getPushNum());
			} else {
				appModel.setPushNum(0);
			}
			appModelList.set(position, appModel);
		}
	}

	/**
	 * 向数据源尾部加入新的app列表
	 * 
	 * @param appModels
	 * @return
	 */
	public ArrayList<AppModel> addList(ArrayList<AppModel> appModels) {
		for (AppModel appModel : appModels) {
			if (!appModelList.contains(appModel)) {
				appModelList.add(appModel);
			}
		}
		return appModelList;
	}

	/**
	 * 更新App的下载安装进度与状态消息数量等属性
	 * 
	 * @param appModels
	 * @return
	 */
	public ArrayList<AppModel> update(AppModel appModel) {
		if (appModelList.contains(appModel)) {
			int position = appModelList.indexOf(appModel);
			AppModel model = appModelList.get(position);
			model.setAppName(appModel.getAppName());
			model.setAppStatus(appModel.getAppStatus());
			model.setProgress(appModel.getProgress());
			//已有的消息数量加上新增的消息数量
			model.setPushNum(model.getPushNum() + appModel.getPushNum());
		} else {
			appModelList.add(appModel);
		}
		return appModelList;
	}
	
    
    /**
     * 是否包含对象
     * 
     * @return
     */
    public void checkContain(AppModel appModel) {
        for (AppModel am : appModelList) {
            if (am.getPackageName().equals(appModel.getPackageName())) {
                // launcherActivity不是空视为包含,并且将赋值入口activity
                if (TextUtils.isEmpty(am.launcherActivity)) {
                    am.setLauncherActivity(appModel.getLauncherActivity());
                }
            }
        }
    }

	/**
	 * 新增一个新的AppModel
	 * 
	 * @param appModel
	 * @return
	 */
	public ArrayList<AppModel> addNew(AppModel appModel) {
		checkContain(appModel);
		if (appModelList.contains(appModel)) {
			int position = appModelList.indexOf(appModel);
			AppModel model = appModelList.get(position);
			model.setAppName(appModel.getAppName());
			model.setAppStatus(appModel.getAppStatus());
			model.setBeUsed(appModel.isBeUsed());
			model.setProgress(appModel.getProgress());
			model.setPushNum(model.getPushNum() + appModel.getPushNum());
		} else {
			int size = appModelList.size();
			int i = size-1;
			int addPosition = size;
			for(;i>=0;i--){
				AppModel model = appModelList.get(i);
				if(!model.isPlaceholder() ){//如果位置i不是占位符
					if(i<size-1){//而且不是列表的最后一个
						//拿到i+1个item,判断是否是占位符
						model = appModelList.get(i+1);
						if(model.isPlaceholder()){
							//如果是占位符,那么新增的app就应该放在这个位置
							addPosition = i+1;
							break;
						}
					}
				}
				
			}
			if( addPosition<size ){
				appModelList.remove(addPosition);
			}
			appModelList.add(addPosition, appModel);
		}
		addPlaceholders();
		return appModelList;
	}

	/**
	 * 往当前页的第一个角标增加一个app model
	 * 
	 * @param appModel
	 * @return
	 */
	public ArrayList<AppModel> addOnFirstByCurrentPage(AppModel appModel) {
		appModelList.add(launcherModel.currentPage * launcherModel.pageItemNum, appModel);
		return appModelList;
	}

	/**
	 * 移除AppModel
	 * 
	 * @param appModel
	 * @return
	 */
	public ArrayList<AppModel> remove(AppModel appModel) {
		if (appModelList.contains(appModel)) {
			appModelList.remove(appModel);
		}
		return appModelList;
	}
	
	/**
	 * 根据角标移除某个数据
	 * 
	 * @param appModel
	 * @return
	 */
	public AppModel remove(int position){
		AppModel model = null;
		if(position<appModelList.size()){
			model = appModelList.remove(position);
		}
		return model;
	}
	
	/**
	 * 将pos位置的app置为占位符
	 * @param pos
	 * @return 返回本页的所有空位item
	 */
	public void placeholder(int pos){
		if(pos<appModelList.size()){
			AppModel model = appModelList.get(pos);
			model.setPlaceholder(true);
			model.setAppName("");
			model.setPushNum(0);
			model.setBeUsed(true);
		}
	}
	
	/**
	 * 根据页的某一position获得page的边界角标
	 * @param position
	 * @return
	 */
	public int[] getPageBoundByPosition(int position){
		//算出pos对应页的第一个item的角标与最后一个item的角标
		int pageItemNum = launcherModel.pageItemNum;
		int positionOnPage = position%pageItemNum;//算出在本页的位置(从0开始,pageItemNum-1表示页末)
		int firstPositionOnPage = position - positionOnPage;
		int endPositeionOnPage = position+(launcherModel.pageItemNum-1-positionOnPage);
		int[] bound = new int[]{firstPositionOnPage, endPositeionOnPage};
		return bound;
	}
	
	/**
	 * 从列表中删除items
	 * @param items
	 */
	public void remove(List<AppModel> items){
		appModelList.removeAll(items);
	}
	
	/**
	 * 将appmodel置为占位符
	 * @param pos
	 */
	public void placeholder(AppModel appModel){
		int indexOf = appModelList.indexOf(appModel);
		if (indexOf!=-1) {
			AppModel model = appModelList.get(indexOf);
			model.setPlaceholder(true);
			model.setAppName("");
			model.setPushNum(0);
			model.setBeUsed(true);
		}
	}

	/**
	 * 存储AppModelList到缓存中去
	 */
	public void save2Cache() {
		if (aCache != null && appModelList != null) {
//			LogUtil.d(TAG, "save2Cache:"+appModelList.toString());
			aCache.put("CacheAppList", appModelList);
		}
	}

	/**
	 * 获取缓存的应用列表,缓存中没有则初始化一个列表
	 * (耗时操作)
	 * @param context
	 * @return
	 */
	public List<AppModel> getCacheList(Context context) {
		List<AppModel> apps = getVisibleList();

		// 获取缓存中的应用列表和当前的应用列表进行比较，进行添加或移除
		aCache = ACache.get(context);
		Object obj = aCache.getAsObject("CacheAppList");
		if (obj == null) {
			appModelList.clear();
			for (AppModel appModel : apps) {
				appModelList.add(appModel);
			}
			// 修改默认排序
			appModelList = sort(appModelList);
		} else {
			appModelList = (ArrayList<AppModel>) obj;
//			LogUtil.d(TAG, "appModelList1:"+appModelList.toString());
//			LogUtil.d(TAG, "apps:"+apps.toString());
			// 移除
			if (appModelList != null && !appModelList.isEmpty()) {
				Iterator<AppModel> iterator = appModelList.iterator();
				while (iterator.hasNext()) {
					AppModel appModel = iterator.next();
					if(appModel.isPlaceholder()){continue;}//不删除占位符
					if (!apps.contains(appModel)) {
						iterator.remove();
					}
				}
			}
//			LogUtil.d(TAG, "appModelList2:"+appModelList.toString());
			// 添加
			if (apps != null && !apps.isEmpty()) {
				for (AppModel app : apps) {
					if (!appModelList.contains(app)) {
						appModelList.add(app);
					}
				}
			}
			// 重置列表app状态
			for (AppModel appModel : appModelList) {
				appModel.setAppStatus(0);
			}
//			LogUtil.d(TAG, "appModelList3:"+appModelList.toString());
		}
		addPlaceholders();
//		LogUtil.d(TAG, "appModelList4:"+appModelList.toString());
		return appModelList;
	}

	/**
	 * 补充占位符
	 * @return 补充的空位集合
	 */
	public List<AppModel> addPlaceholders() {
		int mod = appModelList.size()%launcherModel.pageItemNum;
		List<AppModel> placeholders = new ArrayList<AppModel>();
		if( mod!=0 ){
			//补空位
			int sum = launcherModel.pageItemNum-mod;
			for(int i=0;i<sum;i++){
				AppModel model = new AppModel();
				model.setPlaceholder(true);
				appModelList.add(model);
				placeholders.add(model);
			}
		}
		return placeholders;
	}
	
	/**
	 * 在末页之后增加一页空页的数据
	 */
	public void addEmptyPage(){
		for(int i=0;i<launcherModel.pageItemNum;i++){
			AppModel model = new AppModel();
			model.setPlaceholder(true);
			appModelList.add(model);
		}
	}
	
	
	/**
	 * 根据包名判断是否可显示
	 * @param packageName
	 * @return
	 */
	public boolean displayable(String packageName){
		List<AppModel> visibleList = getVisibleList();
		for(AppModel appModel:visibleList){
			if(TextUtils.equals(appModel.getPackageName(), packageName)){
				return true;
			}
		}
		return false;
	}
	

	/**
	 * 获取可视的应用列表
	 * 
	 * @param context
	 * @return
	 */
	/**
	 * 获取可视的应用列表
	 * 
	 * @param context
	 * @return
	 */
	public List<AppModel> getVisibleList() {
		List<AppModel> apps = new ArrayList<AppModel>();
		PackageManager pm = mContext.getPackageManager();
		Intent intent = new Intent(Intent.ACTION_MAIN, null);
		intent.addCategory(Intent.CATEGORY_LAUNCHER);
		List<ResolveInfo> resolveInfoList = pm.queryIntentActivities(intent, 0);

		for (int i = 0; i < resolveInfoList.size(); i++) {
			ResolveInfo resolveInfo = resolveInfoList.get(i);
			AppModel appModel = new AppModel();

			if ((resolveInfo.activityInfo.applicationInfo.flags & ApplicationInfo.FLAG_SYSTEM) == 0) {
				appModel.setAppName(resolveInfo.loadLabel(pm).toString());
				appModel.setPackageName(resolveInfo.activityInfo.packageName);
				appModel.setLauncherActivity(resolveInfo.activityInfo.name);
				appModel.setIconId(resolveInfo.activityInfo.icon);
				apps.add(appModel);
			} else {
				List<String> blackList = getBlackList();
				if (!blackList.contains(resolveInfo.activityInfo.packageName)) {
					appModel.setAppName(resolveInfo.loadLabel(pm).toString());
					appModel.setPackageName(resolveInfo.activityInfo.packageName);
					appModel.setLauncherActivity(resolveInfo.activityInfo.name);
					appModel.setIconId(resolveInfo.activityInfo.icon);
					apps.add(appModel);
				}
			}
			
		}
		
		return apps;
	}

    public List<String> getBlackList(){
        List<String> blackList = new ArrayList<String>();
        blackList.add("com.woyou.launcher");//商米launcher
        blackList.add("com.android.contacts");//通讯录
        blackList.add("com.android.soundrecorder");// 录音机
        return blackList;
    }
    
    /**
     * 根据包名判断是否是系统应用
     * 
     * @param context
     * @param packageName
     * @return
     */
    public boolean isSystemAppByPkName(String packageName) {

        PackageManager pm = mContext.getPackageManager();
        Intent intent = new Intent(Intent.ACTION_MAIN, null);
        intent.addCategory(Intent.CATEGORY_LAUNCHER);
        intent.setPackage(packageName);
        List<ResolveInfo> resolveInfoList = pm.queryIntentActivities(intent, 0);
        
        ResolveInfo resolveInfo = resolveInfoList.get(0);
        if (resolveInfo.activityInfo.packageName.equals(packageName)) {
            if((resolveInfo.activityInfo.applicationInfo.flags & ApplicationInfo.FLAG_SYSTEM) != 0){
                return true;
            }
        }
        return false;
    }
    
    public boolean canShowDelBtn(AppModel model){
        return !appController.isSystemAppByPkName(model.getPackageName())&&model.getAppStatus() != 2;
    }
    
    /**
     * 根据包名获取该应用的入口列表
     * 
     * @param context
     * @return
     */
    public List<AppModel> getModelListByPackageName(String packageName) {
        List<AppModel> apps = new ArrayList<AppModel>();
        PackageManager pm = mContext.getPackageManager();
        Intent intent = new Intent(Intent.ACTION_MAIN, null);
        intent.setPackage(packageName);
        intent.addCategory(Intent.CATEGORY_LAUNCHER);
        List<ResolveInfo> resolveInfoList = pm.queryIntentActivities(intent, 0);

        for (int i = 0; i < resolveInfoList.size(); i++) {
            ResolveInfo resolveInfo = resolveInfoList.get(i);
            AppModel appModel = new AppModel();
            // 根据包名获取应用入口列表
            if ((resolveInfo.activityInfo.applicationInfo.flags & ApplicationInfo.FLAG_SYSTEM) == 0) {
                appModel.setAppName(resolveInfo.loadLabel(pm).toString());
                appModel.setPackageName(resolveInfo.activityInfo.packageName);
                appModel.setLauncherActivity(resolveInfo.activityInfo.name);
                appModel.setIconId(resolveInfo.activityInfo.icon);
                apps.add(appModel);
            } else {
                List<String> blackList = getBlackList();
                if (!blackList.contains(resolveInfo.activityInfo.packageName)) {
                    appModel.setAppName(resolveInfo.loadLabel(pm).toString());
                    appModel.setPackageName(resolveInfo.activityInfo.packageName);
                    appModel.setLauncherActivity(resolveInfo.activityInfo.name);
                    appModel.setIconId(resolveInfo.activityInfo.icon);
                    apps.add(appModel);
                }
            }
        }
        return apps;
    }



	/**
	 * 交换缓存应用列表的位置
	 * 
	 * @param context
	 * @param oldP
	 * @param newP
	 */
	public synchronized void exchangeDataPostion(int oldP, int newP) {
		if (appModelList.size() - 1 >= oldP) {
			AppModel changeModel = appModelList.get(oldP);
			if (changeModel != null) {
				appModelList.remove(changeModel);
				appModelList.add(newP, changeModel);
			}
		}
	}

	/**
	 * 根据坐标的位置取出列表中的对象
	 * 
	 * @return
	 */
	public AppModel getByPosition(int position) {
		return appModelList.get(position);
	}

	/**
	 * 根据对象返回列表中的位置
	 * 
	 * @return
	 */
	public int indexOf(AppModel appModel) {
		if (appModelList.contains(appModel)) {
			return appModelList.indexOf(appModel);
		}
		return -1;
	}

	@SuppressLint("NewApi")
	public Drawable getIconFromPackageName(String packageName) {
		PackageManager pm = mContext.getPackageManager();
		if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.ICE_CREAM_SANDWICH_MR1) {
			try {
				PackageInfo pi = pm.getPackageInfo(packageName, 0);
				Context otherAppCtx = mContext.createPackageContext(packageName, Context.CONTEXT_IGNORE_SECURITY);
				int displayMetrics[] = { DisplayMetrics.DENSITY_XHIGH, DisplayMetrics.DENSITY_HIGH,
						DisplayMetrics.DENSITY_TV };
				for (int displayMetric : displayMetrics) {
					try {
						Drawable d = otherAppCtx.getResources().getDrawableForDensity(pi.applicationInfo.icon,
								displayMetric);
						if (d != null) {
							return d;
						}
					} catch (Resources.NotFoundException e) {
						continue;
					}
				}
			} catch (Exception e) {
				Log.w(TAG, "error:"+e.getMessage());
			}
		}
		ApplicationInfo appInfo = null;
		try {
			appInfo = pm.getApplicationInfo(packageName, PackageManager.GET_META_DATA);
		} catch (PackageManager.NameNotFoundException e) {
			Log.w(TAG, "error:"+e.getMessage());
			return null;
		}
		return appInfo.loadIcon(pm);
	}
	
	@SuppressLint("NewApi")
	public Drawable getIcon(AppModel model) {
		PackageManager pm = mContext.getPackageManager();
		Intent intent = new Intent();
		intent.setClassName(model.packageName, model.launcherActivity);
		List<ResolveInfo> activities = pm.queryIntentActivities(intent, 0);
		ResolveInfo resolveInfo = activities.get(0);
		Drawable loadIcon = resolveInfo.loadIcon(pm);
		if(loadIcon==null){
			loadIcon = getIconFromPackageName(model.packageName);
		}
		return loadIcon;
	}

	/**
	 * 获取可以打开的应用列表
	 * 
	 * @param context
	 * @return
	 */
	public List<String> getCanOpenList() {
		List<String> canOpens = new ArrayList<String>();

		PackageManager pm = mContext.getPackageManager();
		Intent intent = new Intent(Intent.ACTION_MAIN, null);
		intent.addCategory(Intent.CATEGORY_LAUNCHER);
		List<ResolveInfo> mResolveInfo = pm.queryIntentActivities(intent, 0);
		for (ResolveInfo info : mResolveInfo) {
			String packName = info.activityInfo.packageName;
			if (packName.equals(mContext.getPackageName())) {
				continue;
			}
			canOpens.add(packName);
		}
		return canOpens;
	}

	/**
	 * 通过包名获取应用的名称
	 * 
	 * @param context
	 * @param packageName
	 * @return
	 */
	public String getAppNameByPackage(String packageName) {
		PackageManager pm = mContext.getPackageManager();
		String name = null;
		try {
			name = pm.getApplicationLabel(pm.getApplicationInfo(packageName, PackageManager.GET_META_DATA)).toString();
		} catch (NameNotFoundException e) {
			e.printStackTrace();
		}
		return name;
	}

	/**
	 * 根据应用包名判断该应用是否存在这个列表中
	 * 
	 * @return
	 */
	public boolean containsByName(String packageName) {
		for (AppModel appModel : appModelList) {
			if(appModel.isPlaceholder()){continue;}
			if (appModel.getPackageName().equals(packageName)) {
				return true;
			}
		}
		return false;
	}


	/**
	 * 调整默认排序 系统更新、硬件管家、极盒使用指南、极盒应用市场、设置、调试小助手、打印机固件升级。
	 * 
	 * @param apps
	 * @return
	 */
	private ArrayList<AppModel> sort(ArrayList<AppModel> apps) {
		List<AppModel> newAppModels = new ArrayList<AppModel>();

		if (apps != null && !apps.isEmpty()) {
			// 系统更新
			AppModel otaApp = getByPackageName(apps, "com.woyou.ota");
			if (otaApp != null) {
				newAppModels.add(otaApp);
				apps.remove(otaApp);
			}
			// 硬件管家
			AppModel keeperApp = getByPackageName(apps, "com.woyou.hardwarekeeper");
			if (keeperApp != null) {
				newAppModels.add(keeperApp);
				apps.remove(keeperApp);
			}
			// 极盒使用指南
			AppModel instructionApp = getByPackageName(apps, "com.woyou.instruction");
			if (instructionApp != null) {
				newAppModels.add(instructionApp);
				apps.remove(instructionApp);
			}
			// 极盒使用指南V5
			AppModel instructionV5App = getByPackageName(apps, "com.woyou.instructions_v5");
			if (instructionV5App != null) {
				newAppModels.add(instructionV5App);
				apps.remove(instructionV5App);
			}
			// 极盒应用市场
			AppModel marketApp = getByPackageName(apps, "woyou.market");
			if (marketApp != null) {
				newAppModels.add(marketApp);
				apps.remove(marketApp);
			}
			// 设置
			AppModel settingsApp = getByPackageName(apps, "com.android.settings");
			if (settingsApp != null) {
				newAppModels.add(settingsApp);
				apps.remove(settingsApp);
			}
			// 调试小助手
			AppModel udhApp = getByPackageName(apps, "com.woyou.udh");
			if (udhApp != null) {
				newAppModels.add(udhApp);
				apps.remove(udhApp);
			}
			// 打印机固件升级
			AppModel updaterApp = getByPackageName(apps, "com.woyou.printerupdater");
			if (updaterApp != null) {
				newAppModels.add(updaterApp);
				apps.remove(updaterApp);
			}
			// 蓝牙控制器
			AppModel bluetoothApp = getByPackageName(apps, "com.woyou.bluetoothreceiver");
			if (bluetoothApp != null) {
				newAppModels.add(bluetoothApp);
				apps.remove(bluetoothApp);
			}
			// 将系统应用放在非系统页面的后面
			apps.addAll(newAppModels);
		}
		return apps;
	}

	/**
	 * 判断这个应用列表是否包含这个应用
	 * 
	 * @param apps
	 * @param pageageName
	 * @return
	 */
	public AppModel getByPackageName(List<AppModel> apps, String pageageName) {
		for (AppModel appModel : apps) {
			if(appModel.isPlaceholder()){continue;}
			if (appModel.getPackageName().equals(pageageName)) {
				return appModel;
			}
		}
		return null;
	}
	
	/**
	 * 判断这个应用列表是否包含这个应用
	 * 
	 * @param apps
	 * @param pageageName
	 * @return
	 */
	public AppModel getByPackageName(String pageageName) {
		for (AppModel appModel : appModelList) {
			if(appModel.isPlaceholder()){continue;}
			if (appModel.getPackageName().equals(pageageName)) {
				return appModel;
			}
		}
		return null;
	}
	

	/**
	 * 过滤掉launcherList
	 * 
	 * @return
	 */
	public List<String> getLauncherList() {
		List<String> launcherList = new ArrayList<String>();
		// 我有桌面
		launcherList.add("com.woyou.launcher");
		return launcherList;
	}
	
	/**
	 * 根据角标得到本页最后一个app的角标
	 * @param position
	 * @return
	 */
	public int getLastAppPositionOnPage(int position){
		int pageItemNum = launcherModel.pageItemNum;
		int positionOnPage = (position+1)%pageItemNum;//算出在本页的位置(从1开始,0表示页末)
		if(positionOnPage==0){return position;}//删除页末的app不需要做动画
		int endOfPage = position + pageItemNum - positionOnPage;//算出本页最后一个位置的角标
		int endPosition = -1;//本页最后一个app的位置
		for(int i=endOfPage;i>=position;i--){
			AppModel model = appController.getByPosition(i);
			if(position==i){
				endPosition = i;//如果删除的是最后一个app则不需要做动画
				break;
			}
			if(!model.isPlaceholder()){
				endPosition = i; 
				break;
			}
		}
		return endPosition;
	}
	
	/**
	 * 根据position得到本页最后一个位置的角标
	 * @param position
	 * @return
	 */
	public int getLastPositionOnPage(int position){
		int pageItemNum = launcherModel.pageItemNum;
		int pageNum = position/pageItemNum;//页号
		int firstPositionOnPage = pageNum*pageItemNum;
		int lastPositionOnPage = firstPositionOnPage+pageItemNum-1;
		return lastPositionOnPage;
	}
	
	/**
	 * 根据页号(pagenum)得到page的最后一个角标
	 * @param pageNum
	 * @return
	 */
	public int getLastPositionByPageNum(int pageNum){
		int pageItemNum = launcherModel.pageItemNum;
		int firstPositionOnPage = pageNum*pageItemNum;
		int lastPositionOnPage = firstPositionOnPage+pageItemNum-1;
		return lastPositionOnPage;
	}
	
	/**
	 * 根据角标获得当页第一个角标
	 * @param position
	 * @return
	 */
	public int getFirstPositionOnPage(int position){
		int pageNum = position/launcherModel.pageItemNum;
		int firstPosition = pageNum*launcherModel.pageItemNum;
		return firstPosition;
	}

	/**
	 * 获得当前页第一个占位符的角标
	 * @return
	 */
	public int getFirstPlaceholderOnPage() { 
		int currentPage = launcherModel.currentPage;
		int pageItemNum = launcherModel.pageItemNum;
		int firstPositionOnPage = currentPage*pageItemNum;
		int endPositionOnPage = (currentPage+1)*pageItemNum-1;
		int firstPlaceholderOnPage = -1;
		for(int i=firstPositionOnPage;i<=endPositionOnPage;i++){
			AppModel model = getByPosition(i);
			if(model.isPlaceholder()){
				firstPlaceholderOnPage = i;
				break;
			}
		}
		
		return firstPlaceholderOnPage;
	}
	
	/**
	 * 获得position对应页的第一个占位符的角标
	 * @return
	 */
	public int getFirstPlaceholderOnPage(int position) { 
		int pageNum = position/launcherModel.pageItemNum;//第几页
		int pageItemNum = launcherModel.pageItemNum;
		int firstPositionOnPage = pageNum*pageItemNum;
		int endPositionOnPage = (pageNum+1)*pageItemNum-1;
		int firstPlaceholderOnPage = -1;
		for(int i=firstPositionOnPage;i<=endPositionOnPage;i++){
			AppModel model = getByPosition(i);
			if(model.isPlaceholder()){
				firstPlaceholderOnPage = i;
				break;
			}
		}
		
		return firstPlaceholderOnPage;
	}
	
	/**
	 * 是否可添加到缓存列表
	 * @param packageName
	 * @return
	 */
	public boolean addable(String packageName){
		// 过滤桌面luncher
		List<String> list = getLauncherList();
		if(list.contains(packageName)){return false;}
		
		List<String> canOpenList = getCanOpenList();
		if(!canOpenList.contains(packageName)){return false;}
		
		return true;
	}

	/**
	 * 是否包含
	 * @param appModel
	 */
	public boolean contains(AppModel appModel) {
		return appModelList.contains(appModel);
	}
	
	public int getSize(){
		return appModelList.size();
	}

	/**
	 * 插入一个数据
	 * @param model
	 * @param lastPositionOnPage
	 */
	public void add( int position, AppModel model) {
		appModelList.add(position, model);
	}
	
	/**
	 * 打开一个应用
	 * 
	 * @param appModel
	 */
	public void openApp(AppModel appModel) {

		Intent intent = new Intent();
		PackageManager packageManager = mContext.getPackageManager();
		intent = packageManager.getLaunchIntentForPackage(appModel.getPackageName());
		String packageName = appModel.getPackageName();
		String launcherActivity = appModel.getLauncherActivity();
		if (intent != null && !TextUtils.isEmpty(packageName) && !TextUtils.isEmpty(launcherActivity)) {
			ComponentName comp = new ComponentName(packageName, launcherActivity);
			intent.setComponent(comp);
			intent.setPackage(null);
			intent.setAction("android.intent.action.MAIN");
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
			mContext.startActivity(intent);

			// 记录当前被打开的应用
			setOpenAppModel(appModel);
			// 刷新Layout
			if (!appModel.isBeUsed()) {
				appModel.setBeUsed(true);
//				Intent refreshIntent = new Intent();
//				refreshIntent.setAction(BroadcastConstants.HANDLE_REFRESH_RECEIVER);
//				refreshIntent.putExtra("AppModel", appModel);
//				mContext.sendBroadcast(refreshIntent);
			}
		}

	}
	
}
