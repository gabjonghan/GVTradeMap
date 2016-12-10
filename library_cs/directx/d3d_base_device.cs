/*-------------------------------------------------------------------------

 Direct3D
 Deviceの初期化を担当する
 継承して사용すること
 デバイスリセット時の処理付き

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing.Imaging;
using System;
using System.Runtime.InteropServices;

using Utility;
using System.Windows.Forms;
using System.Diagnostics;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace directx
{
	public abstract class d3d_base_device : IDisposable
	{
		public enum CreateType{
			BestPerformance,				// デバイスの能力から最適なものを선택する
			SoftwareVertexProcessing,		// 頂点をCPUが変換する
		};
	
		private System.Windows.Forms.Form			m_form;

		private int									m_adapter_index;
		private Device								m_d3d_device;
		private Caps								m_caps;
		private CreateFlags							m_create_flags;
		private PresentParameters					m_present_params;
		private DeviceType							m_device_type;
		private CreateType							m_create_type;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public System.Windows.Forms.Form form		{	get{	return m_form;				}}
		public int adpter_index						{	get{	return m_adapter_index;		}}
		public Device device						{	get{	return m_d3d_device;		}}
		public Caps caps							{	get{	return m_caps;				}}
		public CreateFlags create_flags				{	get{	return m_create_flags;		}}
		public PresentParameters present_params		{	get{	return m_present_params;	}}
		public DeviceType device_type				{	get{	return m_device_type;		}}
		public CreateType create_type				{	get{	return m_create_type;		}}

		/*-------------------------------------------------------------------------
		 Deviceを作成する
		 作成できなかったときの例外はスルーするので呼び出し側で対応すること
		 CreateType.BestPerformanceで作成される
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param)
		{
			Create(form, param, CreateType.BestPerformance, DeviceType.Hardware);
		}

		/*-------------------------------------------------------------------------
		 Deviceを作成する
		 作成できなかったときの例外はスルーするので呼び出し側で対応すること
		 CreateTypeを指定する
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param, CreateType create_type)
		{
			Create(form, param, create_type, DeviceType.Hardware);
		}

		/*-------------------------------------------------------------------------
		 Deviceを作成する
		 作成できなかったときの例外はスルーするので呼び出し側で対応すること
		 CreateType, DeviceTypeを指定する
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param, CreateType create_type, DeviceType device_type)
		{
			// 그리기対象
			m_form				= form;
			// デバイスタイプ
			m_device_type		= device_type;
			// 作成방법
			m_create_type		= create_type;

			// デフォルトのアダプタ번호
			m_adapter_index		= Manager.Adapters.Default.Adapter;
			// デバイスの能力を取得する
			m_caps				= Manager.GetDeviceCaps(m_adapter_index, device_type);

			// 作成パラメータ
			m_present_params	= param;

			// 
			m_create_flags		= 0;
			switch(create_type){
			case CreateType.BestPerformance:
				// 最適なものを選ぶ
				if(m_caps.DeviceCaps.SupportsHardwareTransformAndLight){
					// ハードウェアで頂点変換に対応している
					m_create_flags	|= CreateFlags.HardwareVertexProcessing;
				}else{
					// ハードウェアで頂点変換に対応していない
					m_create_flags	|= CreateFlags.SoftwareVertexProcessing;
				}
				if(   (m_caps.DeviceCaps.SupportsPureDevice)
					&&(m_caps.DeviceCaps.SupportsHardwareTransformAndLight) ){
					// ピュアデバイスである
					m_create_flags	|= CreateFlags.PureDevice;
				}
				break;
			case CreateType.SoftwareVertexProcessing:
				// 정점처리をCPUが行う
				m_create_flags	|= CreateFlags.SoftwareVertexProcessing;
				break;
			default:
				throw new Exception("CreateTypeの指定が不正");
			}

			// デバイスを作成
			m_d3d_device	= new Device(	m_adapter_index, device_type, form,
											m_create_flags, m_present_params);
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public virtual void Dispose()
		{
			if(m_d3d_device != null)	m_d3d_device.Dispose();
			m_d3d_device	= null;
		}

		/*-------------------------------------------------------------------------
		 clear
		---------------------------------------------------------------------------*/
		public void Clear(System.Drawing.Color color)
		{
			m_d3d_device.Clear(ClearFlags.ZBuffer | ClearFlags.Target , color ,1.0f, 0);
		}
		public void Clear(ClearFlags flags, System.Drawing.Color color)
		{
			m_d3d_device.Clear(flags, color ,1.0f, 0);
		}

		/*-------------------------------------------------------------------------
		 그리기開始
		---------------------------------------------------------------------------*/
		public virtual bool Begin()
		{
			if(!OnDeviceLostException())	return false;
			m_d3d_device.BeginScene();
			return true;
		}

		/*-------------------------------------------------------------------------
		 그리기終了
		---------------------------------------------------------------------------*/
		public virtual void End()
		{
			m_d3d_device.EndScene();
		}

		/*-------------------------------------------------------------------------
		 present
		---------------------------------------------------------------------------*/
		public virtual bool Present()
		{
			if(m_d3d_device == null)	return true;

			try{
				m_d3d_device.Present();
			}catch(DeviceLostException){
				// デバイスロスト
				OnDeviceLostException();
			}
			catch (DriverInternalErrorException)
			{
				return false;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 デバイスロスト
		---------------------------------------------------------------------------*/
		protected virtual bool OnDeviceLostException()
		{
			int	result;

			// リセット可能かどうかをチェック
			if(!m_d3d_device.CheckCooperativeLevel(out result)){
				// リセット可能ならリセット
				if(result == (int)ResultCode.DeviceNotReset){
					m_d3d_device.Reset(m_present_params);
				}else if (result == (int)ResultCode.DeviceLost){
					// まだリセットできなければ, しばらくスリープ
					System.Threading.Thread.Sleep(20);
					return false;
				}
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 指定されたフォーマットが사용可能かどうかを得る
		 現在のデバイスを対象に調べる
		 통상텍스쳐作成時にこの関数で결과を得るべきではない
		 初期化時に결과を知っておくこと
		---------------------------------------------------------------------------*/
		public bool CheckDeviceFormat(Usage usage, ResourceType resource_type, DepthFormat format)
		{
			if(m_d3d_device == null)	return false;
			Format	af		= (m_present_params.Windowed)
								? m_d3d_device.DisplayMode.Format		// 윈도우모드時
								: m_present_params.BackBufferFormat;	// フルスクリーン時
			return Manager.CheckDeviceFormat(	m_adapter_index,
												m_device_type,
												af,
												usage,
												resource_type,
												format);
		}
		public bool CheckDeviceFormat(Usage usage, ResourceType resource_type, Format format)
		{
			if(m_d3d_device == null)	return false;
			Format	af		= (m_present_params.Windowed)
								? m_d3d_device.DisplayMode.Format		// 윈도우모드時
								: m_present_params.BackBufferFormat;	// フルスクリーン時
			return Manager.CheckDeviceFormat(	m_adapter_index,
												m_device_type,
												af,
												usage,
												resource_type,
												format);
		}
	}
}
