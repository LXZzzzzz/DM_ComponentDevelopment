using System.Collections.Generic;

namespace ToolsLibrary.EquipPart
{
    //机场功能
    public interface IAirPort
    {
        bool checkComeIn();
        void comeIn(string equipId);
        List<string> GetAllEquips();
        void goOut(string equipId);
    }

    //火源点功能
    public interface ISourceOfAFire
    {
        //模板资源动态初始化功能
        void fireInit(float fs, float pd, float csrsmj, string id, string colorCode, string chooseColoeCode);
        void waterPour(float time, float squareMeasure, float weight);
        bool getTaskProgress();
        void getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float tszl);
    }

    //灾区点功能
    public interface IDisasterArea
    {
        //模板资源动态初始化功能
        void disasterInit(string id, int personNum, int type, string colorCode, string chooseColoeCode);
        void airdropGoods(float time, float squareMeasure);
        int getWoundedPersonnelType();
        int rescuePerson(int maxrescuePersonNum);
        bool getTaskProgress(out int currentNum, out int maxNum);
    }

    //安置点功能: 放置物资和安置人员
    public interface IRescueStation
    {
        void goodsPour(float weight);
        int placementOfPersonnel(int personNum);
        void getResData(out float firstTime, out float totalWeight, out int totalPerson);
        bool getTaskProgress(out int currentPersonNum, out int maxPersonNum, out float currentGoodsNum, out float maxGoodsNum);
    }

    //医院功能：和安置点一样
    public interface IHospital
    {
        void goodsPour(float weight);
        int placementOfPersonnel(int personNum);
        void getResData(out float firstTime, out float totalWeight, out int totalPerson);
        bool getTaskProgress(out int currentPersonNum, out int maxPersonNum, out float currentGoodsNum, out float maxGoodsNum);
    }

    //任务点功能
    public interface ITaskProgress
    {
        //现在把任务组件去掉，包含任务的组件继承我即可
        string getAssociationAssemblyId();
        bool getTaskProgress(out string progressInfo, out float progressNum);
    }
}