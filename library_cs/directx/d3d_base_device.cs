/*-------------------------------------------------------------------------

 Direct3D
 Device 의 초기화를 담당한다
 상속해서 사용할 것
 디바이스 리셋 시의 처리 포함

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
			BestPerformance,				// 디바이스 성능에서 최적인 것을 선택한다
			SoftwareVertexProcessing,		// 정점을 CPU 가 변환한다
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
		 Device 를 작성한다
		 작성하지 못했을 때의 예외는 그대로 던지므로 호출하는 쪽에서 대응할 것
		 CreateType.BestPerformance 로 작성된다
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param)
		{
			Create(form, param, CreateType.BestPerformance, DeviceType.Hardware);
		}

		/*-------------------------------------------------------------------------
		 Device 를 작성한다
		 작성하지 못했을 때의 예외는 그대로 던지므로 호출하는 쪽에서 대응할 것
		 CreateType 을 지정한다
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param, CreateType create_type)
		{
			Create(form, param, create_type, DeviceType.Hardware);
		}

		/*-------------------------------------------------------------------------
		 Device 를 작성한다
		 작성하지 못했을 때의 예외는 그대로 던지므로 호출하는 쪽에서 대응할 것
		 CreateType, DeviceType 을 지정한다
		---------------------------------------------------------------------------*/
		public void Create(System.Windows.Forms.Form form, PresentParameters param, CreateType create_type, DeviceType device_type)
		{
			// 그리기대상
			m_form				= form;
			// 디바이스 타입
			m_device_type		= device_type;
			// 작성방법
			m_create_type		= create_type;

			// 기본 어댑터 번호
			m_adapter_index		= Manager.Adapters.Default.Adapter;
			// 디바이스 성능을 취득한다
			m_caps				= Manager.GetDeviceCaps(m_adapter_index, device_type);

			// 작성 파라미터
			m_present_params	= param;

			// 
			m_create_flags		= 0;
			switch(create_type){
			case CreateType.BestPerformance:
				// 최적인 것을 고른다
				if(m_caps.DeviceCaps.SupportsHardwareTransformAndLight){
					// 하드웨어가 정점 변환을 지원한다
					m_create_flags	|= CreateFlags.HardwareVertexProcessing;
				}else{
					// 하드웨어가 정점 변환을 지원하지 않는다
					m_create_flags	|= CreateFlags.SoftwareVertexProcessing;
				}
				if(   (m_caps.DeviceCaps.SupportsPureDevice)
					&&(m_caps.DeviceCaps.SupportsHardwareTransformAndLight) ){
					// 퓨어 디바이스이다
					m_create_flags	|= CreateFlags.PureDevice;
				}
				break;
			case CreateType.SoftwareVertexProcessing:
				// 정점 처리를 CPU 가 수행한다
				m_create_flags	|= CreateFlags.SoftwareVertexProcessing;
				break;
			default:
				throw new Exception("CreateType 지정이 올바르지 않음");
			}

			// 디바이스를 작성
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
		 그리기 시작
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
				// 디바이스 로스트
				OnDeviceLostException();
			}
			catch (DriverInternalErrorException)
			{
				return false;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 디바이스 로스트
		---------------------------------------------------------------------------*/
		protected virtual bool OnDeviceLostException()
		{
			int	result;

			// 리셋 가능한지 확인
			if(!m_d3d_device.CheckCooperativeLevel(out result)){
				// 리셋 가능하면 리셋
				if(result == (int)ResultCode.DeviceNotReset){
					m_d3d_device.Reset(m_present_params);
				}else if (result == (int)ResultCode.DeviceLost){
					// 아직 리셋할 수 없으면 잠시 대기
					System.Threading.Thread.Sleep(20);
					return false;
				}
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 지정한 포맷을 사용할 수 있는지 여부를 얻는다
		 현재 디바이스를 대상으로 조사한다
		 통상 텍스쳐 작성 시에 이 함수로 결과를 얻어서는 안 된다
		 초기화 시에 결과를 알아 둘 것
		---------------------------------------------------------------------------*/
		public bool CheckDeviceFormat(Usage usage, ResourceType resource_type, DepthFormat format)
		{
			if(m_d3d_device == null)	return false;
			Format	af		= (m_present_params.Windowed)
								? m_d3d_device.DisplayMode.Format		// 윈도우 모드일 때
								: m_present_params.BackBufferFormat;	// 풀스크린일 때
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
								? m_d3d_device.DisplayMode.Format		// 윈도우 모드일 때
								: m_present_params.BackBufferFormat;	// 풀스크린일 때
			return Manager.CheckDeviceFormat(	m_adapter_index,
												m_device_type,
												af,
												usage,
												resource_type,
												format);
		}
	}
}
