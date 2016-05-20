package sunmi.launcher;

import java.io.Serializable;



/**
 * app应用类
 * 
 */
@SuppressWarnings("serial")
public class AppModel implements Serializable {
	 
	/**是否是空位**/
	private boolean isPlaceholder = false;
	/**
	 * 应用名
	 */
	private String appName;
	/**
	 * 包名
	 */
	public String packageName;
	
	/**
	 * 启动的activity name
	 */
	public String launcherActivity;
	
	/**
	 * icon id
	 */
	public int iconId;
	
	/**
	 * · 新下载（或更新）完毕、尚未打开过的app的标志位
	 */
	private boolean beUsed = false;
	/**
	 * 未读推送数量
	 */
	private int pushNum = 0;
	/**
	 * 1：安装成功 2:下载成功 3:下载失败/被卸载 4:下载中 5:默认为等待状态 6:磁盘空间不足 7:安装失败 8:更新 9:暂停 10:未安装
	 */
	private int appStatus = 0;
	/**
	 * icon图片的Url
	 */
	private String iconUrl;
	/**
	 * app下载中状态的当前进度
	 */
	private int progress = 0;

	public boolean isPlaceholder() {
		return isPlaceholder;
	}

	public void setPlaceholder(boolean isPlaceholder) {
		this.isPlaceholder = isPlaceholder;
		if(isPlaceholder){
			int random=(int)(Math.random()*900)+100; 
			this.packageName = System.currentTimeMillis()+"_"+random;
		}
	}

	public String getAppName() {
		return appName;
	}

	public void setAppName(String appName) {
		this.appName = appName;
	}

	public String getPackageName() {
		return packageName;
	}

	public void setPackageName(String packageName) {
		this.packageName = packageName;
	}

	public boolean isBeUsed() {
		return beUsed;
	}

	public void setBeUsed(boolean beUsed) {
		this.beUsed = beUsed;
	}

	public int getPushNum() {
		return pushNum;
	}

	public void setPushNum(int pushNum) {
		this.pushNum = pushNum;
	}

	public int getAppStatus() {
		return appStatus;
	}

	public void setAppStatus(int appStatus) {
		this.appStatus = appStatus;
	}

	public String getIconUrl() {
		return iconUrl;
	}

	public void setIconUrl(String iconUrl) {
		this.iconUrl = iconUrl;
	}

	public int getProgress() {
		return progress;
	}

	public void setProgress(int progress) {
		this.progress = progress;
	}

	public String getLauncherActivity() {
		return launcherActivity;
	}

	public void setLauncherActivity(String launcherActivity) {
		this.launcherActivity = launcherActivity;
	}
	
	
	
	public int getIconId() {
		return iconId;
	}

	public void setIconId(int iconId) {
		this.iconId = iconId;
	}

	@Override
	public String toString() {
		return "AppModel [isPlaceholder=" + isPlaceholder + ", appName="
				+ appName + ", packageName=" + packageName
				+ ", launcherActivity=" + launcherActivity + ", beUsed="
				+ beUsed + ", pushNum=" + pushNum + ", appStatus=" + appStatus
				+ ", iconUrl=" + iconUrl + ", progress=" + progress + "]";
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime
				* result
				+ ((launcherActivity == null) ? 0 : launcherActivity.hashCode());
		result = prime * result
				+ ((packageName == null) ? 0 : packageName.hashCode());
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		AppModel other = (AppModel) obj;
		if (launcherActivity == null) {
			if (other.launcherActivity != null)
				return false;
		} else if (!launcherActivity.equals(other.launcherActivity))
			return false;
		if (packageName == null) {
			if (other.packageName != null)
				return false;
		} else if (!packageName.equals(other.packageName))
			return false;
		return true;
	}
	
	




}
