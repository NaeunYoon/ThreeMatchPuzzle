using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���⿡ ���õ� ������
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
    //�ܺο��� ����ؾ��ϱ� ������ �� �̹����� ������Ƽ�� ������ش�
    //���� ����� spriteRenderer�� �����´�
    public SpriteRenderer BlockImage
    {
        get
        {
            return _BlockImage;
        }
    }

    //���� �̵���Ű���� �÷����� �ο찪�� �����ؾ���
    public int Column { set; get;}  //���� ���� �÷�
    public int Row { set; get;} //���� ���� �ο�

    //���� �����̴��� �����ִ��� ���¸� �����ϴ� ������Ƽ�� �����
    public BLOCKSTATE State { set;get; } = BLOCKSTATE.STOP;

    //��������Ʈ �̹����� Ÿ���� �������ش� => �پ��� �̹����� �÷��� �ÿ� Ȱ���ϱ� ���ؼ� Ÿ�� ������Ƽ�� ����� �̹����� �����Ѵ�
    public int Type { set; get; }


    void Start()
    {
        
    }

    public float Width  //�� �ʺ� �����ϴ� ������Ƽ
    {
        set;get;
    }
    private Vector3 _movePos; //���� �̵��� ��ġ���� ����
    public Vector3 MovePos  //_movePos�� private�̱� ������ property�� ���ؼ� �����ϵ��� ����
    {
        get => _movePos;
        set => _movePos = value;
    }

    private DIRECTION _direct; //�̵����� ����
    public float Speed { set; get; } = 0.01f;
    /// <summary>
    /// �� �����̰� �ϴ� �Լ� (������ ���)
    /// ���� �����Ǵ� ��ũ��Ʈ�̱� ������ ���Ⱚ�� ���ڷ� �����Ѵ�
    /// ���� ���� width��ŭ �����δ�
    /// </summary>
    public void Move(DIRECTION direct)
    {
        //���� ���� �����̰� �ִµ� ����� ������ ���Ͻ�Ų��.
        if(State == BLOCKSTATE.MOVE)
        {
            return;
        }
        switch(direct)
        {
            case DIRECTION.LEFT:
                {
                    _movePos = transform.position; //���� ��ġ���� ����
                    _movePos.x -= Width; //���� ��ġ�� x ������ ���� �ʺ� ���� �̵��Ѵ�
                    State = BLOCKSTATE.MOVE;
                    _direct = DIRECTION.LEFT;   //�̵��� ������ ����
                    //�̵��̶�� ���� ���� ��ġ������ �ٸ� ��ġ�� ������ �ð����� �Ź� �̵��ϴ� �Լ��� ȣ��Ǵ°��̶� ������Ʈ���� ȣ��
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
                        //���ʹ������� �̵��� ����
                        transform.Translate(Vector3.left * Speed, Space.Self);
                        //��� ������ϴ��� ���ǹ����� �����ش�
                        //���� ��ġ�� ���� ��� �����̴ٰ� movepos���� �۾����� ���� ��ġ�� movePos�� �ȴ�
                        //movePos ������ �̵��Ѵ�
                        if(transform.position.x <= _movePos.x)
                        {
                            transform.position = _movePos;
                            State = BLOCKSTATE.STOP;
                        }
                    }
                break;

                case DIRECTION.RIGHT:
                    {
                        //�����ʹ������� �̵��� ����
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
