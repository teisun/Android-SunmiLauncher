using System.Collections;
using UnityEngine;

[System.Serializable]
public class App {

	//是否是空位
	public bool isPlaceholder = false;

	/**
	 * 应用名
	 */
	public string appName;

	/**
	 * 包名
	 */
	public string packageName;

	/**
	 * 启动的activity name
	 */
	public string launcherActivity;

	/**
	 * · 新下载（或更新）完毕、尚未打开过的app的标志位
	 */
	public bool beUsed = false;

	/**
	 * 未读推送数量
	 */
	public int pushNum = 0;

	/**
	 * 1：安装成功 2:下载成功 3:下载失败/被卸载 4:下载中 5:默认为等待状态 6:磁盘空间不足 7:安装失败 8:更新 9:暂停 10:未安装
	 */
	public int appStatus = 0;

	/**
	 * icon图片的Url
	 */
	public string iconUrl;

	/**
	 * app下载中状态的当前进度
	 */
	public int progress = 0;

	public void SetIsPlaceholder(bool isPlaceholder){
		this.isPlaceholder = isPlaceholder;
		if(isPlaceholder){
			int random = Random.Range (900, 999);
			this.packageName = System.DateTime.Now.Millisecond+"_"+random;
			this.launcherActivity = this.packageName;
		}
	}

	public override int GetHashCode(){
		int prime = 31;
		int result = 1;
		result = prime
			* result
			+ ((launcherActivity == null) ? 0 : launcherActivity.GetHashCode());
		result = prime * result
			+ ((packageName == null) ? 0 : packageName.GetHashCode());
		return result;
	}


	public override bool Equals(System.Object obj){
		if (this == obj) {
			return true;
		}
		if (obj == null) {
			return false;
		}
		if (GetType() != obj.GetType ()) {
			return false;
		}
		App other = (App)obj;
		if (launcherActivity == null) {
			if (other.launcherActivity != null) {
				return false;
			}
		} else if (!launcherActivity.Equals (other.launcherActivity)) {
			return false;
		}
		if (packageName == null) {
			if (other.packageName != null) {
				return false;
			}
		} else if (!packageName.Equals (other.packageName)) {
			return false;
		}

		return true;
	}

}
