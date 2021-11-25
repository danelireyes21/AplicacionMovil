package crc641386efd8132b1b80;


public class VerVehiculosActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("AppMovilExamen3.VerVehiculosActivity, AppMovilExamen3", VerVehiculosActivity.class, __md_methods);
	}


	public VerVehiculosActivity ()
	{
		super ();
		if (getClass () == VerVehiculosActivity.class)
			mono.android.TypeManager.Activate ("AppMovilExamen3.VerVehiculosActivity, AppMovilExamen3", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
