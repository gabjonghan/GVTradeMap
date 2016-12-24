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
			SoftwareVertexProcessing,		// 頂点をCPUが변환する
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
		 Deviceを작성함
		 작성できなかったときの例외はスルーするので呼び出し側で대응すること
		 CreateType.BestPerformanceで작성される
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param)
		{
			Create(form, param, CreateType.BestPerformance, DeviceType.Hardware);
		}

		/*-------------------------------------------------------------------------
		 Deviceを작성함
		 작성できなかったときの例외はスルーするので呼び出し側で대응すること
		 CreateTypeを지정する
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param, CreateType create_type)
		{
			Create(form, param, create_type, DeviceType.Hardware);
		}

		/*-------------------------------------------------------------------------
		 Deviceを작성함
		 작성できなかったときの例외はスルーするので呼び出し側で대응すること
		 CreateType, DeviceTypeを지정する
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param, CreateType create_type, DeviceType device_type)
		{
			// 그리기대상
			m_form				= form;
			// デバイス타입
			m_device_type		= device_type;
			// 작성방법
			m_create_type		= create_type;

			// デフォルトのアダプタ번호
			m_adapter_index		= Manager.Adapters.Default.Adapter;
			// デバイスの能力を取得する
			m_caps				= Manager.GetDeviceCaps(m_adapter_index, device_type);

			// 작성パラメータ
			m_present_params	= param;

			// 
			m_create_flags		= 0;
			switch(create_type){
			case CreateType.BestPerformance:
				// 最適なものを選ぶ
				if(m_caps.DeviceCaps.SupportsHardwareTransformAndLight){
					// ハードウェアで頂点변환に대응している
					m_create_flags	|= CreateFlags.HardwareVertexProcessing;
				}else{
					// ハードウェアで頂点변환に대응していない
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
				throw new Exception("CreateTypeの지정が不正");
			}

			// デバイスを작성
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
		 그리기종료
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

			// リセット가능かどうかをチェック
			if(!m_d3d_device.CheckCooperativeLevel(out result)){
				// リセット가능ならリセット
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
		 지정されたフォーマットが사용가능かどうかを得る
		 現在のデバイスを대상に調べる
		 통상텍스쳐작성時にこの関수で결과を得るべきではない
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
