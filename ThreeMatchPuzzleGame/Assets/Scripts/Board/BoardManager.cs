using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.Mathematics;
using UnityEngine;


/// <summary>
/// 마우스 이동방향을 계산하는 열거형
/// </summary>
public enum MouseMoveDirection
{
     MOUSEMOVERIGHT,
     MOUSEMOVELEFT,
     MOUSEMOVEUP,
     MOUSEMOVEDOWN
};

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
    /// 

    //Updata 문에서 블럭에 포함된 점의 좌표를 찍기 위해 만드는 멤버필드 ( 컬럼과 로우만큼 for문을 돌릴 때 사용)
    private int _Column = 0;
    private int _Row = 0;

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

        //받은 col과row를  사용하여 블럭의 좌표를 알아낸다
        _Column = column;
        _Row = row;

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

                //블럭에 이름을 전달 (어느게 찍혔는지 확인)
                _GameBoard[co, ro].name = $"Block[{co},{ro}]";
                
                
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

    //블럭을 끌어당길 때의 각 구하기
    //마우스를 클릭했을 때 시작 위치를 정하는 start 떼었을 때 종료 위치를 정하는 end
    private Vector3 _StartPos = Vector3.zero;   //마우스 클릭 시 시작좌표
    private Vector3 _EndPos = Vector3.zero; //마우스 클릭 후의 좌표
    bool _IsOver = false;   //클릭된 블럭이 있는지 저장
    //클릭된 오브젝트 저장
    GameObject _ClickObject;    //클릭된 블럭 참조값 저장
    bool _MouseClick = false;   //마우스가 클릭상태인지 아닌지 체크
    float _MoveDistance = 0.01f; //민감도 조절(시작점에서 끝점 거리의 판단 기준값)

    /// <summary>
    /// 클릭된 상태에서 마우스 이동 시 호출
    /// </summary>
    void MouseMove()
    {
        Debug.Log("MouseMove");
        //감도체크
        //두 벡터사이의 거리를 구한다
        float diff = Vector2.Distance(_StartPos, _EndPos);
        //조건 
        //움직인 거리가 내가 설정한 MoveDistance 보다 클 때 처리, MoveDistance : 감도 0.01f
        // 클릭된 오브젝트가 있을 때
        if(diff > _MoveDistance && _ClickObject != null) 
        {
            //어느쪽 방향으로 움직임이 일어났는지 계산
            // 마우스 방향 enum, 마우스 이동 시 방향을 계산하는 함수, 방향 계산 뒤에 그 사이의 각도를 구하는 함수
            MouseMoveDirection dir = CalculateDirection();
            Debug.Log("Direction "+dir);
        }
    }
    /// <summary>
    /// 마우스 이동 시 이동한 방향을 계산하는 함수
    /// </summary>
    /// <param name="mouseMove"></param>
    /// <returns></returns>
    private MouseMoveDirection CalculateDirection ()
    {
        float angle = CalculateAngle(_StartPos, _EndPos);   //z축을 기준으로 회전하는 각도값을 구한다
        /*
         up벡터를 0으로 뒀을 떄, 시계방향으로 0 45 90 135 180 225 270 315 360 
         */
        if (angle > 315.0f && angle <= 360 || angle >= 0 && angle < 45.0f)
        {
            return MouseMoveDirection.MOUSEMOVEUP;
        }
        else if(angle >= 45f && angle<135f)
        {
            //화면 반대편에서 보기 때문에 left로 바뀐다
            return MouseMoveDirection.MOUSEMOVELEFT;
        }
        else if(angle >= 135f && angle <225f)
        {
            return MouseMoveDirection.MOUSEMOVEDOWN;
        }
        else if(angle>=225f && angle<315f)
        {
            //화면 반대편에서 보기 때문에 left로 바뀐다
            return MouseMoveDirection.MOUSEMOVERIGHT;
        }
        else
        {
            //어차피 위에서 걸릴거라 else 의미는 없음 ( 그냥 right 를 else 해서 리턴해도 됨
            return MouseMoveDirection.MOUSEMOVEDOWN;
        }
    }
    /// <summary>
    /// 두 벡터 사이의 각도를 구하는 함수
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private float CalculateAngle(Vector3 from, Vector3 to)
    {
        //블럭의 네 부분 각도를 구해 그 바운더리에 들어온 위치를 통해 방향을 정한다
        //to 에서 from 좌표값을 뺴면 from 에서 to로 향하는 좌표값을 만들 수 있다
        //up벡터와 to 벡터 사이의 각도를 구하는 함수가 FromToRotation이다.
        //우리가 쳐다보는 좌표가 z 이기 때문에, 회전 각도 중에서도 z 값만 취한다 => 
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    void Update()
    {
        /*
         게임 : 게임 메인루프가 돌아가고 -> 입력 확인을 하고 -> 입력에 따른 프로세싱 처리하고 -> 화면에 랜더링 한다
         */

        //마우스 버튼이 눌렸을 때 
        if(Input.GetMouseButtonDown(0))
        {
            //마우스 클릭한 상태로 변경
            _MouseClick = true;

            //Bounds 오류나는 경우 이걸로 쓰기 
            Vector3 pos = Input.mousePosition;
            //pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));

            //StartPos와 EndPos 를 마우스가 처음 클릭한 좌표로 저장
             _EndPos =_StartPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));   //클릭이 되었을 때 위치를 시작점과 끝점 위치값으로 초기화
            //z값을 쓰지 않으니 0으로 초기화
            _EndPos.z = _StartPos.x = 0f;


            //2D에서는 마우스로 하는게 더 편함 (3D는 광선을 발사해서 사용-> 무거움)

            //마우스 버튼이 눌렸을 때, 눌린 위치값을 가져온다
            //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Pos " + pos);

            //스크린상에서 찍은 포지션 값이 출력된다.
            //그러나 그 좌표에 어떤 블럭이 있는지 확인해야함 
            //블럭의 사각영역 안에 우리가 찍은 점이 포함되어있는지 블럭의 갯수만큼 돌려야 한다
            //게임보드에 블럭을 넣어놨기 때문에 거기 있는 블럭만큼 돌리면 되기 때문에 col과 row 값을 기록하는 멤버필드를 만든다

            //클릭이 되었는지 안 되었는지 체크하는 변수


             _IsOver = false;
            for (int i = 0; i < _Column; i++)
            {
                for (int j   = 0; j < _Row; j++)
                {
                    //클릭한 좌표값과 겹쳐지는 블럭을 찾는 것
                    //먼저 블럭이 있는지를 체크한다 (없는 경우도 있음)


                    if (_GameBoard[i,j]!= null)
                    {
                        //클릭이 되었는지 안 되었는지 체크하는 변수
                        //각각의 블럭들은 블럭 컴포넌트의 부모컴포넌트에 있는 블럭에 접근하고 그건 또 자식들의 spriterenderer에 접근해서
                        //사각형의 bounds 를 가져온다. bound안에 a마우스 클릭한 월드 좌표의 위치가 포함되는지 여부를 bool값으로 전달한다

                        _IsOver = _GameBoard[i,j].GetComponent<Block>().BlockImage.GetComponent<SpriteRenderer>().bounds.Contains(pos);
                        //isOver = true;

                        //만약에 오브젝트의 사이즈로 하려면 콜라이더를 넣어준다.


                    }

                    if(_IsOver==true)
                    {
                        //찍힌 블럭의 이름이 출력된다.
                        Debug.Log("ClickObj = "+_GameBoard[i,j].name);

                        //클릭한 오브젝트의 참조값을 클릭오브젝트 멤버필드에 할당
                        _ClickObject = _GameBoard[i, j];

                        //중첩되어있는 for문을 한번에 빠져나가기 (그렇지 않으면 break 를 두 번 써줘야 함)
                        goto LoopExit;
                    }

                }
            }
        LoopExit:;

        }
        //마우스 버튼을 놨을 때
        if(Input.GetMouseButtonUp(0))
        {
            _ClickObject= null;
            _MouseClick= false;

        }
        //마우스가 클릭된 상태에서 마우스가 어떤 방향이든 변화가 있는지
        //1. 마우스 클릭을 TRUE 로 처리 (클릭된 상태에서 드래그를 했는지 안했는지 처리)
        //2. X축으로 이동했는지 (변화량이 있는지 => 어느쪽이든 상관 없음. 0이면 변화량이 없음)
        if(_MouseClick == true&&(Input.GetAxis("Mouse X")<0) || (Input.GetAxis("Mouse X")>0)
            || (Input.GetAxis("Mouse Y"))<0 || (Input.GetAxis("Mouse Y") > 0))
        {
            //변화가 있는 마우스 커서의 위치를 가져와서 을 ENDPOS에 저장
            //클릭된 상태에서 마우스가 움직인 것. 그 움직인 위치값을 endPos가 된다
            //클릭점은 startPos가 되고 내가조금이라도 움직이면 그걸 endpos로 정한다
            Vector3 pos = Input.mousePosition;
            _EndPos = Camera.main.ScreenToWorldPoint(new Vector3( pos.x, pos.y, 10f));
            _EndPos.z = 0f;

            //마우스가 움직였을때 처리하는 함수
            MouseMove();
        }
    }
}
