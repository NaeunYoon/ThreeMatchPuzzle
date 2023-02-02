using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _BlockPrefab;

    private Vector3 _screenPos; //��ũ�� 0,0���� ���� ��ǥ�� ����
    float _ScreenWidth; //��ũ�� ����
    float _BlockWidth;  //�� �ϳ��� ���̸� �����ϱ� ���� ����
    float _ScreenHeight;    //��ũ�� ����
    //����ó�� (2�� �ʿ���(x,y)
    private float _Xmargin = 0.5f; //x�� ����
    private float _Ymargin = 2f; //y�� ����

    float _Scale = 0f;  //���� �����ϰ� ����
    [SerializeField] private GameObject Parent;

    //�� ������ ������ ( ��ư ��ũ��Ʈ�� �� �����̴� �뵵) : 2���� �迭 ���
    public GameObject[,] _GameBoard;

    void Start()
    {
        //��ũ��0,0��ǥ�� ���� �������� ��ǥ������ ��ȯ
        _screenPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10f));
        //+
        _screenPos.y = -_screenPos.y;
        Debug.Log(_screenPos);

        //��ũ���� ���̴� �Ʊ� ���� ��ũ�� 0,0 ��ǥ�� x ���� ��( x ��ǥ�� - �̱� ������ ������ ���Ѵ�)
        _ScreenWidth = Mathf.Abs(_screenPos.x * 2);
        //�� �������� ���� �� ������Ʈ�� �ִ� �� �̹��� ������Ƽ�� �����ؼ� ���� ����� �����´�
        //�ȼ����� 100���� ������ ���� : ����Ƽ�󿡼� �ȼ� 100�� 1��. size.x �� �ȼ��������ε� �̰� 100���� ������
        //3d �������� ����Ƽ ����(����)�� �ٲ۰��̴�. �ȼ��� 1000�� ��� 3d�������� ���̴� 10�� �ȴ�.
        // pixel per unit (100�ȼ��� 1����) => 1���ͷ� ����� �ȴ�.=> 100���� ������ ����Ƽ�� ������ �����
        _BlockWidth = _BlockPrefab.GetComponent<Block>().BlockImage.sprite.rect.size.x/100;




        MakeBoard(10,10);
    }
    /// <summary>
    /// ����ġ instantiate -> transform.position ->
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    void MakeBoard(int column, int row)
    {
        float width = _ScreenWidth -(_Xmargin * 2); //��µǴ� �ʺ� ( 5.6�̵�. ��ũ�� x �� 2.8�̰� ��ü ��ũ�� �ʺ� ���� ��)
        //=> x ������ ���ʿ� �ֱ� ������ ���ϱ� 2 ���ش� (y�� �ϳ��� �������ָ� ��)

        //���� �ʺ� �ϳ��� �����ϳ� ��ü �ʺ�� row �� ������ ���� �޶�����
        float blockWidth = _BlockWidth * row;   //���� ��ü ��� �ʺ� (���� row���� ���� ��ü ����� �޶�����)
        //���� ������ ���� �����ؾ��Ѵ� => ���� 5,5 �� ��ũ�� ȭ�� �ȿ� ����� ������ 10,10 �̸� ȭ�� ������ �Ѿ��
        //=> �ֳĸ� �� �ʺ�� row ���� ���� �޶����� ������
        //��ũ�� �ʺ� ���� �� ���� ��������? ���ʺ�� ��ü �ʺ�� ������ ������ ���� ���Ѵ� -> �� ������ ����
        _Scale = width / blockWidth; //���� ������ �� 

        //��� �� ������ ���� ���带 ����� ( �迭�� ũ�⸦ ������)
        _GameBoard = new GameObject[column, row];

        for (int co = 0; co < column; co++)
        {
            for (int ro = 0; ro < row; ro++)
            {
                //���� ������ �޾Ƽ� Instantiate �Ѵ�.
                //GameObject blockObj = Instantiate(_BlockPrefab);
                //���� �� �����ϰ����� ���� �������� �������ش�

                //2���� �迭�� �ٲ��ش�
                _GameBoard[co,ro] = Instantiate(_BlockPrefab);

                //�������ؼ� �θ� ����
                _GameBoard[co, ro].transform.SetParent(Parent.transform);

                _GameBoard[co, ro].transform.localScale = new Vector3(_Scale, _Scale, 0f);
                //���� �����ϰ��� ���������� ��ũ���� �� �������� ����.


                //���� ���� �ڸ��� ��������� ������ ��ġ�� �������ش� (����������)
                //�� ó�� ��ġ�κ��� �����ϸ鼭 ��������� ������ �������� ���� ����
                //�츮�� ���ϴ°� ���ϴ����� ������ ���̱� ������ y���� -�� �ش�
                //screenpos�� 0.0 ��ǥ�� �˾Ƴ� ���� �»�� ��ǥ�� �����ش�
                //���ʿ� ro,co ù��° ��ǥ�� 1�̱� ������ ������ �������� ����
                //�׸��� 0.5f �����ؼ� ȭ�鿡 ������ �� (���� �߸��� �ʰ�)
                //blockObj.transform.position = new Vector3(_screenPos.x+ ro + 0.5f, _screenPos.y-co-0.5f, 0.0f);

                //blockwidth ���� blockscale���� �������ش�
                //blockObj.transform.position = new Vector3(_screenPos.x + ro * (_BlockWidth * _Scale) + 0.5f, 
                //                                          _screenPos.y - co * (_BlockWidth * _Scale) - 0.5f, 0.0f);

                //screenpos�� �»���ε� x������ ���� => x������ŭ �������� ��µȴ� (�ű⼭���� �� ��ġ)
                //0.5�� ���Ƿ� ��ġ�ߴµ� ���� ������ ���� ������ �޶��� ( ���� ������ ���� Ŀ���� �۾����� �ϱ� ������
                //�������� �����ش� (_BlockWidth*_Scale/2) => x �ุŭ ����
                //y������ ������ �Ʒ��� �������� ������ ���̳ʽ� ���ش�. 
                _GameBoard[co, ro].transform.position = new Vector3(_screenPos.x + _Xmargin + ro * (_BlockWidth * _Scale) + (_BlockWidth *_Scale)/2f,
                                          _screenPos.y - _Ymargin - co * (_BlockWidth * _Scale) - (_BlockWidth * _Scale)/2f, 0.0f);

                //�� ��ũ��Ʈ�� �ִ� width���ٰ� ���� �ʺ� �����Ѵ� (�Űܾ� �ϱ� ����)
                _GameBoard[co, ro].GetComponent<Block>().Width = (_BlockWidth * _Scale); //���� �ʺ��� ���� ����
                _GameBoard[co, ro].GetComponent<Block>().MovePos = _GameBoard[co, ro].transform.position;   //�� ���� ��ġ���� ����

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
