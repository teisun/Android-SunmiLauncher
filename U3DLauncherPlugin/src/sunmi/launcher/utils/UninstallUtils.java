package sunmi.launcher.utils;

import sunmi.launcher.AppModel;
import sunmi.launcher.BroadcastConstants;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;

/**
 * 
 * 卸载Dialog的工具类
 * 
 * @versionCode 1 <每次修改提交前+1>
 */
public class UninstallUtils {
	private static final String TAG = "UninstallDialogUtils";
	
	/**
	 * 是否可以静默卸载
	 * @param context
	 * @param appModel
	 * @return
	 */
	public static boolean canSilenceUninstall(Context context, String packageName){
		Uri packageURI = Uri.parse("package:" + packageName);
		Intent intent = new Intent(BroadcastConstants.ACTION_DELETE_HIDE, packageURI);
		return ProcessUtils.isIntentAvailable(context, intent);
	}
	
	/**
	 * 静默卸载
	 * @param context
	 * @param appModel
	 */
	public static void silenceUninstall(Context context, AppModel appModel){
		Uri packageURI = Uri.parse("package:" + appModel.getPackageName());
		Intent intent = new Intent(BroadcastConstants.ACTION_DELETE_HIDE, packageURI);
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		context.startActivity(intent);
	}
	
	/**
	 * 卸载
	 * @param context
	 * @param appModel
	 */
	public static void uninstall(Context context, AppModel appModel){
		Uri packageURI = Uri.parse("package:" + appModel.getPackageName());
		Intent intent = new Intent(Intent.ACTION_DELETE, packageURI);
		context.startActivity(intent);
	}

	/**
	 * 移除应用广播
	 * @param mContext
	 * @param appModel
	 */
	private static void broadcastUninstall(Context mContext, AppModel appModel) {
		Intent i = new Intent();
		i.setAction(BroadcastConstants.APP_REMOVE);
		i.putExtra("packageName", appModel.getPackageName());
		mContext.sendBroadcast(i);
	}
}
