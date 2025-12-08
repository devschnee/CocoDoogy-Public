using System.Collections.Generic;

[System.Serializable]
public class TableMetaList
{
    public List<TableMetaEntry> entries;
}
//json으로 사용하기 위한 랩핑 코드

[System.Serializable]
public class TableMetaEntry
{
    public string name;
    public string type;
    public string url;
}
//데이터테이블 클래스 구조
//todo : 데이터 테이블 추가 될 시 json파일에 name, type 올릴것
//name은 Assets/_Proj/Data/CSV에 넣은 .csv 파일 이름을 그대로 사용
//스크립터블오브젝트화 해야 한다면 type 에 "SO", 아닐시 "CSV_ONLY"