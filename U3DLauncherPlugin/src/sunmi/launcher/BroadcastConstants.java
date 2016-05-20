package sunmi.launcher;

/**
 * 全局常量
 * 
 * @author 荣
 *
 */
public class BroadcastConstants {

	
	/**
	 * 静默卸载Action
	 */
	public static final String ACTION_DELETE_HIDE = "android.intent.action.DELETE.HIDE";
	/**
	 * 新增应用广播
	 */
	public static final String NEWAPP = "NEWAPP";
	/**
	 * 恢复停用应用广播
	 */
	public static final String RESUMESTOPAPP = "RESUMESTOPAPP";
	/**
	 * 更新应用广播
	 */
	public static final String APP_UPDATE = "APP_UPDATE";
	/**
	 * 卸载应用广播
	 */
	public static final String APP_REMOVE = "APP_REMOVE";
	/**
	 * 监听home按钮广播
	 */
	public static final String SYSTEM_HOME_KEY = "homekey";
	/**
	 * 局部刷新某一个app view的广播
	 */
	public static final String APP_REFRESH = "APP_REFRESH";
	/**
	 * 接收推送数量来更新红色角标
	 */
	public static final String PUSHNUM_RECEIVER = "PUSHNUM_RECEIVER";
	/**
	 * app开始安装的广播
	 */
	public static final String APP_INSTALL = "APP_INSTALL";
	/**
	 * 安装任务被移除的广播
	 */
	public static final String INSTALL_TASK_REMOVE = "INSTALL_TASK_REMOVE";
	/**
	 * aidl Service的action
	 */
	public static String APPMARKET_ACTION = "woyou.appmarket.service";
	/**
	 * aidl service对应的package name
	 */
	public static String APPMARKET_PACKNAME = "woyou.market";
}
