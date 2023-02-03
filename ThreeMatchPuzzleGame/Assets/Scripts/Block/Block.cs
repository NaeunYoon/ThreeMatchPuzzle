using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방향에 관련된 열거형
/// </summary>
public enum DIRECTION
{
    LEFT,
    RIGHT,
    UP,
    DOWN
};

public enum BLOCKSTATE
{
    STOP,
    MOVE
}
public class Block : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _BlockImage;
    //외부에서 사용해야하기 때문에 블럭 이미지를 프로퍼티로 만들어준다
    //블럭의 사이즈를 spriteRenderer로 가져온다
    public SpriteRenderer BlockImage
    {
        get
        {
            return _BlockImage;
        }
    }

    //블럭을 이동시키려면 컬럼값과 로우값을 저장해야함
    public int Column { set; get;}  //현재 블럭의 컬럼
    public int Row { set; get;} //현재 블럭의 로우

    //블럭이 움직이는지 멈춰있는지 상태를 저장하는 프로퍼티를 만든다
    public BLOCKSTATE State { set;get; } = BLOCKSTATE.STOP;

    //스프라이트 이미지의 타입을 지정해준다 => 다양한 이미지를 플레이 시에 활용하기 위해서 타입 프로퍼티를 만들고 이미지를 배정한다
    public int Type { set; get; }


    void Start()
    {
        
    }

    public float Width  //블럭 너비 저장하는 프로퍼티
    {
        set;get;
    }
    private Vector3 _movePos; //블럭의 이동할 위치값을 저장
    public Vector3 MovePos  //_movePos가 private이기 때문에 property를 통해서 접근하도록 만듬
    {
        get => _movePos;
        set => _movePos = value;
    }

    private DIRECTION _direct; //이동방향 저장
    public float Speed { set; get; } = 0.01f;
    /// <summary>
    /// 블럭 움직이게 하는 함수 (열거형 사용)
    /// 블럭에 장착되는 스크립트이기 떄문에 방향값을 인자로 전달한다
    /// 블럭은 블럭의 width만큼 움직인다
    /// </summary>
    public void Move(DIRECTION direct)
    {
        //현재 블럭이 움직이고 있는데 명령이 들어오면 리턴시킨다.
        if(State == BLOCKSTATE.MOVE)
        {
            return;
        }
        switch(direct)
        {
            case DIRECTION.LEFT:
                {
                    _movePos = transform.position; //현재 위치값을 저장
                    _movePos.x -= Width; //현재 위치의 x 값에서 블럭의 너비를 뺴서 이동한다
                    State = BLOCKSTATE.MOVE;
                    _direct = DIRECTION.LEFT;   //이동할 방향을 저장
                    //이동이라는 것은 현재 위치값에서 다른 위치로 일정한 시간마다 매번 이동하는 함수가 호출되는것이라 업데이트에서 호출
                }
                break;
            case DIRECTION.RIGHT:
                {
                    _movePos = transform.position;
                    _movePos.x += Width;
                    State = BLOCKSTATE.MOVE;
                    _direct = DIRECTION.RIGHT;
                }
                break;
            case DIRECTION.UP:
                {
                    _movePos = transform.position;
                    _movePos.y += Width;
                    State = BLOCKSTATE.MOVE;
                    _direct = DIRECTION.UP;
                }
                break;
            case DIRECTION.DOWN:
                {
                    _movePos = transform.position;
                    _movePos.y -= Width;
                    State = BLOCKSTATE.MOVE;
                    _direct = DIRECTION.DOWN;
                }
                break;
        }
    }

    void Update()
    {
        if (State == BLOCKSTATE.MOVE)
        {
            switch(_direct)
            {
                case DIRECTION.LEFT:
                    {
                        //왼쪽방향으로 이동을 시작
                        transform.Translate(Vector3.left * Speed, Space.Self);
                        //어디서 멈춰야하는지 조건문으로 정해준다
                        //현재 위치의 값이 계속 움직이다가 movepos보다 작아지면 현재 위치가 movePos가 된다
                        //movePos 까지만 이동한다
                        if(transform.position.x <= _movePos.x)
                        {
                            transform.position = _movePos;
                            State = BLOCKSTATE.STOP;
                        }
                    }
                break;

                case DIRECTION.RIGHT:
                    {
                        //오른쪽방향으로 이동을 시작
                        transform.Translate(Vector3.right*Speed, Space.Self);
                        if(transform.position.x >=_movePos.x)
                        {
                            transform.position = _movePos;
                            State = BLOCKSTATE.STOP;
                        }
                    }
                break;

                case DIRECTION.UP:
                    {
                        transform.Translate(Vector3.up*Speed, Space.Self);
                        if(transform.position.y>=_movePos.y)
                        {
                            transform.position = _movePos;
                            State = BLOCKSTATE.STOP;
                        }
                    }
                break;

                case DIRECTION.DOWN:
                    {
                        transform.Translate(Vector3.down*Speed, Space.Self);
                        if(transform.position.y <= _movePos.y)
                        {
                            transform.position = _movePos;
                            State = BLOCKSTATE.STOP;
                        }
                    }
                break;
            }
        }
    }
}
