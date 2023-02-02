using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _BlockPrefab;

    private Vector3 _screenPos; //스크인 0,0점의 월드 좌표값 저장
    float _ScreenWidth; //스크린 넓이
    float _BlockWidth;  //블럭 하나의 넓이를 조정하기 위한 변수
    float _ScreenHeight;    //스크린 높이
    //여백처리 (2개 필요함(x,y)
    private float _Xmargin = 0.5f; //x축 여백
    private float _Ymargin = 2f; //y축 여백

    float _Scale = 0f;  //블럭의 스케일값 저장
    [SerializeField] private GameObject Parent;

    //블럭 정보를 저장함 ( 버튼 스크립트로 블럭 움직이는 용도) : 2차원 배열 사용
    public GameObject[,] _GameBoard;

    void Start()
    {
        //스크린0,0좌표를 월드 공간상의 좌표값으로 변환
        _screenPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10f));
        //+
        _screenPos.y = -_screenPos.y;
        Debug.Log(_screenPos);

        //스크린의 넓이는 아까 구한 스크린 0,0 좌표의 x 값의 곱( x 좌표가 - 이기 때문에 절댓값을 구한다)
        _ScreenWidth = Mathf.Abs(_screenPos.x * 2);
        //블럭 프리팹을 통해 블럭 컴포넌트에 있는 블럭 이미지 프로퍼티에 접근해서 가로 사이즈를 가져온다
        //픽셀값을 100으로 나누는 이유 : 유니티상에서 픽셀 100이 1임. size.x 가 픽셀사이즈인데 이걸 100으로 나눠서
        //3d 공간상의 유니티 단위(유닛)로 바꾼것이다. 픽셀이 1000인 경우 3d공간상의 길이는 10이 된다.
        // pixel per unit (100픽셀당 1유닛) => 1미터로 여기면 된다.=> 100으로 나눠서 유니티의 단위로 만든다
        _BlockWidth = _BlockPrefab.GetComponent<Block>().BlockImage.sprite.rect.size.x/100;




        MakeBoard(10,10);
    }
    /// <summary>
    /// 블럭배치 instantiate -> transform.position ->
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    void MakeBoard(int column, int row)
    {
        float width = _ScreenWidth -(_Xmargin * 2); //출력되는 너비 ( 5.6이됨. 스크린 x 는 2.8이고 전체 스크린 너비를 구한 것)
        //=> x 마진이 양쪽에 있기 때문에 곱하기 2 해준다 (y는 하나만 적용해주면 됨)

        //블럭의 너비 하나는 일정하나 전체 너비는 row 의 갯수에 따라 달라진다
        float blockWidth = _BlockWidth * row;   //블럭의 전체 출력 너비 (블럭의 row값에 따라 전체 사이즈가 달라진다)
        //블럭의 스케일 값을 저장해야한다 => 블럭이 5,5 면 스크린 화면 안에 출력이 되지만 10,10 이면 화면 밖으로 넘어간다
        //=> 왜냐면 블럭 너비는 row 값에 따라 달라지기 때문에
        //스크린 너비에 블럭을 다 집어 넣으려면? 블럭너비로 전체 너비로 나눠서 스케일 값을 구한다 -> 블럭 사이즈 조정
        _Scale = width / blockWidth; //블럭의 스케일 값 

        //행과 열 값으로 게임 보드를 만든다 ( 배열의 크기를 정해줌)
        _GameBoard = new GameObject[column, row];

        for (int co = 0; co < column; co++)
        {
            for (int ro = 0; ro < row; ro++)
            {
                //블럭의 갯수를 받아서 Instantiate 한다.
                //GameObject blockObj = Instantiate(_BlockPrefab);
                //위의 블럭 스케일값으로 블럭의 스케일을 조정해준다

                //2차원 배열로 바꿔준다
                _GameBoard[co,ro] = Instantiate(_BlockPrefab);

                //지저분해서 부모 만듬
                _GameBoard[co, ro].transform.SetParent(Parent.transform);

                _GameBoard[co, ro].transform.localScale = new Vector3(_Scale, _Scale, 0f);
                //블럭의 스케일값을 조정했지만 스크린에 다 들어오지는 않음.


                //블럭이 같은 자리에 만들어지기 때문에 위치를 변경해준다 (정방향으로)
                //맨 처음 위치로부터 증가하면서 만들어지기 때문에 우상단으로 블럭이 퍼짐
                //우리가 원하는건 우하단으로 퍼지는 것이기 때문에 y값을 -로 준다
                //screenpos의 0.0 좌표를 알아낸 다음 좌상단 좌표로 맞춰준다
                //애초에 ro,co 첫번째 좌표가 1이기 때문에 기준이 그쪽으로 잡힘
                //그리고 0.5f 보정해서 화면에 나오게 함 (블럭이 잘리지 않게)
                //blockObj.transform.position = new Vector3(_screenPos.x+ ro + 0.5f, _screenPos.y-co-0.5f, 0.0f);

                //blockwidth 값과 blockscale값을 보정해준다
                //blockObj.transform.position = new Vector3(_screenPos.x + ro * (_BlockWidth * _Scale) + 0.5f, 
                //                                          _screenPos.y - co * (_BlockWidth * _Scale) - 0.5f, 0.0f);

                //screenpos가 좌상단인데 x마진을 더함 => x마진만큼 떨어져서 출력된다 (거기서부터 블럭 배치)
                //0.5를 임의로 배치했는데 블럭의 갯수에 따라 보정이 달라짐 ( 블럭이 갯수에 따라 커졌다 작아졌다 하기 때문에
                //고정값을 정해준다 (_BlockWidth*_Scale/2) => x 축만큼 보정
                //y마진은 위에서 아래로 내려가기 때문에 마이너스 해준다. 
                _GameBoard[co, ro].transform.position = new Vector3(_screenPos.x + _Xmargin + ro * (_BlockWidth * _Scale) + (_BlockWidth *_Scale)/2f,
                                          _screenPos.y - _Ymargin - co * (_BlockWidth * _Scale) - (_BlockWidth * _Scale)/2f, 0.0f);

                //블럭 스크립트에 있는 width에다가 블럭의 너비를 대입한다 (옮겨야 하기 때문)
                _GameBoard[co, ro].GetComponent<Block>().Width = (_BlockWidth * _Scale); //블럭의 너비값을 블럭에 전달
                _GameBoard[co, ro].GetComponent<Block>().MovePos = _GameBoard[co, ro].transform.position;   //블럭 생성 위치값을 저장

                //_GameBoard[co, ro].GetComponent<Block>().Move(DIRECTION.LEFT);

                
            }
        }
    }
    #region uiButton
    public void MoveBlockLeft()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.LEFT);
        }
    }

    public void MoveBlockRight()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.RIGHT);
        }
    }

    public void MoveBlockUp()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.UP);
        }
    }

    public void MoveBlockDown()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.DOWN);
        }
    }
    #endregion
    void Update()
    {
        
    }
}
