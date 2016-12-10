namespace gvtrademap_cs.Properties {
    
    
    // このクラスでは설정クラスでの特定のイベントを処理することができます:
    //  SettingChanging イベントは, 설정値が변경される前に発生します. 
    //  PropertyChanged イベントは, 설정値が변경された後に発生します. 
    //  SettingsLoaded イベントは, 설정値が読み込まれた後に発生します. 
    //  SettingsSaving イベントは, 설정値が保存される前に発生します. 
    internal sealed partial class Settings {
        
        public Settings() {
            // // 설정の保存と변경のイベント ハンドラを追加するには, 以下の行のコメントを해제します:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // SettingChangingEvent イベントを処理するコードをここに追加してください. 
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // SettingsSaving イベントを処理するコードをここに追加してください. 
        }
    }
}
