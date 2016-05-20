package sunmi.launcher.utils;

import sunmi.launcher.AppModel;
import sunmi.launcher.BroadcastConstants;
import android.content.Context;
import android.content.Intent;

public class BroadcastUtils {


	/**
	 * 移除应用广播
	 * @param mContext
	 * @param appModel
	 */
	public static void uninstallApp(Context mContext, AppModel appModel) {
		Intent i = new Intent();
		i.setAction(BroadcastConstants.APP_REMOVE);
		i.putExtra("packageName", appModel.getPackageName());
		mContext.sendBroadcast(i);
	}
}
