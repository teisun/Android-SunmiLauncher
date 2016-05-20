package sunmi.launcher;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;

/**
 * 检测app安装和卸载广播
 * 
 */
public class AppStatusReceiver extends BroadcastReceiver {

	@Override
	public void onReceive(Context context, Intent intent) {
		AppController appController = AppController.getInstance(context);
		if (intent.getAction().equals(Intent.ACTION_PACKAGE_ADDED)) {
			String packageName = intent.getDataString().substring(8);
			// 安装应用成功
			Intent i = new Intent();
			i.setAction(BroadcastConstants.NEWAPP);
			i.putExtra("packageName", packageName);
			context.sendBroadcast(i);
		}
		if (intent.getAction().equals(Intent.ACTION_PACKAGE_REMOVED)) {
			// 卸载应用成功
			String packageName = intent.getDataString().substring(8);
			Intent i = new Intent();
			i.setAction(BroadcastConstants.APP_REMOVE);
			i.putExtra("packageName", packageName);
			context.sendBroadcast(i);

		}
//		if (intent.getAction().equals(Intent.ACTION_PACKAGE_REPLACED)) {
//			// 过滤非初始化更新的应用
//			String packageName = intent.getDataString().substring(8);
//			Intent i = new Intent();
//			i.setAction(BroadcastConstants.APP_UPDATE);
//			i.putExtra("packageName", packageName);
//			context.sendBroadcast(i);
//		}
		
		if (intent.getAction().equals(Intent.ACTION_PACKAGE_CHANGED)) {
			String packageName = intent.getDataString().substring(8);
			PackageManager packageManager = context.getPackageManager();
			intent = packageManager.getLaunchIntentForPackage(packageName);
			Intent i = new Intent();
			//如果intent==null则说明packageName被停用
			if(intent==null){
				i.setAction(BroadcastConstants.APP_REMOVE);
				i.putExtra("packageName", packageName);
				context.sendBroadcast(i);
			}else{
				//如果该app已经在列表里则无需添加
				boolean contains = appController.containsByName(packageName);
				if(contains)return;
				
				i.setAction(BroadcastConstants.RESUMESTOPAPP);
				i.putExtra("packageName", packageName);
				context.sendBroadcast(i);
			}
		}
	}

}
