/** 
 * @Title:  LauncherModel.java 
 * @author:  longtao.li
 * @data:  2016年3月3日 下午12:45:52 <创建时间>
 * 
 * @history：<以下是历史记录>
 *
 * @modifier: <修改人>
 * @modify date: 2016年3月3日 下午12:45:52 <修改时间>
 * @log: <修改内容>
 *
 * @modifier: <修改人>
 * @modify date: 2016年3月3日 下午12:45:52 <修改时间>
 * @log: <修改内容>
 */
package sunmi.launcher;


/** 
 * Launcher模型
 * @author longtao.li 
 * @versionCode 1 <每次修改提交前+1>
 */
public class LauncherModel {
	
	private static LauncherModel instance;
	
	private LauncherModel(){}
	
	public static LauncherModel getInstance(){
		if(instance == null){
			instance = new LauncherModel();
		}
		return instance;
	}
	
	public static final String V8 = "V8";
	public static final String H10 = "H10";
	public static final String H14 = "H14";
	public static final String V1 = "V1";
	
	public  String model = V1;
	public  int currentPage = 0;//当前页
	public  int pageItemNum;//每页多少个item
	public  int rowNum;//行数
	public  int columnNum;//列数
	public int screenWidth;
	public int screenHeight;
	public int itemWidth;
	public int itemHeight;
	public float iconScaleOnScreenW; //icon相对于屏幕宽的比例
	public int iconWidth;//icon 的宽高
	public int fontSize;
	public int statusBarHeight;
	public int proportion;
	

	
}
