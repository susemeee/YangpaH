namespace YangpaH
{
    /// <summary>
    /// Wrapps static data and constants.
    /// </summary>
    abstract class YangpaConstants
    {
        public const string Class = "Class";
        public const string Students = "Students";
        public const string CaptainPresent = "CaptainPresent";
        public const string Captains = "Captains";
        public const string ClassEnd = "End Class";
        public const string DB_Tablename = "activity";

        private static System.Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public readonly static string Version = v.Major + "." + v.Minor + "." + v.Build;
        public readonly static string AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public const string AppTitle = "양파양파해↗";
        public const string MSG_CLS_NOT_SELECTED = "반이 선택되지 않았습니다.";
        public const string MSG_INS_NOT_SELECTED = "수업이 선택되지 않았습니다.";
        public const string MSG_CAP_NOT_SIX = "조장은 6명이 되어야 합니다.";
        public const string MSG_INV_CONF = "반 정보 파일이 잘못되었습니다. \nconfig.yangpa 파일을 지우고 다시 만들거나 파일을 직접 수정해 주세요.";
        public const string MSG_COL_IS_NOT_UNIQ = "DB에 중복되는 이름이 존재합니다..";
        public const string MSG_DB_NOT_EXIST = "그런 이름을 가진 DB가 없습니다.. 버그??";
        public const string MSG_ENTER_NAME = "이름을 입력하지 않았습니다.";
        public const string MSG_SET_MEMBER_BEFORE_MOVE = "양파를 옮기기 전에 조원을 설정해 주세요.";
        public const string MSG_ASK_SCRAMBLE_WARN = "양파가 배치되어 있는 상태에서 조원을 섞으려고 합니다.\n계속하시겠습니까?";
        public const string MSG_CAP_IS_NOT_MEMBER = "조장은 반 인원에 포함되어야 합니다.";
        public const string MSG_DB_CLS_MISMATCH = "DB의 학생 리스트와 반의 학생 리스트가 맞지 않습니다. \n반 인원 설정을 확인해 주세요.";
        public const string MSG_WILL_MISMATCH_DB = "DB가 생성된 경우 반 인원 설정을 바꾸게 되면 오류가 발생할 수 있습니다. \n계속하시겠습니까?";
        public const string MSG_EXC_EXP_SUCCESS = "내보내기가 완료되었습니다.";
        public const string MSG_CONFIRM_DELETE_INST = "이 수업을 삭제하시겠습니까?";
        public const string MSG_EMPTY_VALUE = "입력하지 않은 값이 있거나 값이 잘못되었습니다.";
        public const string MSG_FIRST_TIME = "처음 실행하시는 경우, 왼쪽 위에 있는 반 선택 리스트에서 반을 추가해 주세요.";
        public const string MSG_BUG_NO_DUP_NAME = "Bug #3 동명이인이 처리되지 않았습니다.";
        public const string MSG_HAS_DUPLICATE = "중복되는 이름(동명이인) 은 서로 구별되어야 합니다.";
        public const string MSG_INST_NO_MEMBER_IN_CLASS = "반 인원에 포함되지 않은 이름이 존재합니다. \n이는 엑셀 파일로 내보낼 때 오류를 불러올 수 있습니다.\n계속하시겠습니까?";
        public const string MSG_SHOULD_USE_MIRROR = "DB가 존재하지 않습니다. \n백업된 파일을 사용하시겠습니까?";
    }

}
