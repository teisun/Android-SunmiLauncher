package sunmi.launcher.utils;

import android.app.Activity;
import android.util.DisplayMetrics;
import android.util.Log;
/**
 * 一些常用的系统配置信息 获取自定义的配置信息 类
 *
 */
public class Adaptation{

	public static final int NOTE1 = 1;
	
	public static int screenHeight=0;
	public static int screenWidth=0;
	public static float screenDensity=0;
	public static int densityDpi = 0;
	public static int version = Integer.valueOf(android.os.Build.VERSION.SDK_INT);
	
	public static final int SCREEN_9_16 = 1;//9:16
	public static final int SCREEN_3_4 = 2;//3:4
	public static final int SCREEN_4_3 = 3;//4:3
	public static final int SCREEN_16_9 = 4;//16:9
	public static int proportion = SCREEN_9_16;
	
	public static final float PROPORTION_9_16 = 0.56f;//9:16
	public static final float PROPORTION_3_4 = 0.75f;//3:4
	public static final float PROPORTION_4_3 = 1.33f;//4:3
	public static final float PROPORTION_16_9 = 1.77f;//16:9
	
	public static final float AVERAGE1 = 0.655f;//(9:16+3:4)/2
	public static final float AVERAGE2 = 1.04f;//(3:4+4:3)/2
	public static final float AVERAGE3 = 1.55f;//(4:3+16:9)/2
	
	public static void init(Activity context) {
			if(screenDensity==0||screenWidth==0||screenHeight==0){
				DisplayMetrics dm = new DisplayMetrics();
				context.getWindowManager().getDefaultDisplay().getMetrics(dm);
				Adaptation.screenDensity = dm.density;
				Adaptation.screenHeight = dm.heightPixels;
				Adaptation.screenWidth = dm.widthPixels;
				Adaptation.densityDpi = dm.densityDpi;
			}
			
			float proportionF = (float)screenWidth/(float)screenHeight;
			
			if( proportionF<=AVERAGE1 ){
				proportion = SCREEN_9_16;
			}else if(proportionF>AVERAGE1&&proportionF<=AVERAGE2){
				proportion = SCREEN_3_4;
			}else if(proportionF>AVERAGE2&&proportionF<=AVERAGE3){
				proportion = SCREEN_4_3;
			}else if(proportionF>AVERAGE3){
				proportion = SCREEN_16_9;
			}
			
			Log.i("SCREEN CONFIG", "screenHeight:"+screenHeight+";screenWidth:"+screenWidth
					+";screenDensity:"+screenDensity+";densityDpi:"+densityDpi);
			
	}
    
}