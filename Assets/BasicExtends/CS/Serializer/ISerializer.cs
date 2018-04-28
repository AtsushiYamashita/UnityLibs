namespace BasicExtends.SerializeImp
{
    using System.Collections.Generic;
    using System;

    public interface ISerializer
    {
        ByteList GetTypeList(List<string> mTypeIndex);
        void SetSerializer(string type, Func<object, ByteList> func);
        void SetDeserializer(string type, Func<ByteList, object> func);
        CheckedRet<object> Deserial( ByteList bytes);
        ByteList ToSerial(object obj);
    }
}