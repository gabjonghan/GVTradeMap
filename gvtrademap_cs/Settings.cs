namespace gvtrademap_cs.Properties {
	
	
	// 이 클래스에서는 설정 클래스의 특정 이벤트를 처리할 수 있습니다:
	//  SettingChanging 이벤트는 설정값이 변경되기 전에 발생합니다.
	//  PropertyChanged 이벤트는 설정값이 변경된 뒤에 발생합니다.
	//  SettingsLoaded 이벤트는 설정값을 읽어들인 뒤에 발생합니다.
	//  SettingsSaving 이벤트는 설정값이 저장되기 전에 발생합니다.
	internal sealed partial class Settings {
		
		public Settings() {
			// // 설정 저장과 변경의 이벤트 핸들러를 추가하려면, 아래 행의 주석을 해제합니다:
			//
			// this.SettingChanging += this.SettingChangingEventHandler;
			//
			// this.SettingsSaving += this.SettingsSavingEventHandler;
			//
		}
		
		private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
			// SettingChangingEvent 이벤트를 처리하는 코드를 여기에 추가해 주십시오.
		}
		
		private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
			// SettingsSaving 이벤트를 처리하는 코드를 여기에 추가해 주십시오.
		}
	}
}
