﻿/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace sg.protocol.msg_web_api
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class MsgAnsGetUserGameChapter : TBase
  {
    private sg.protocol.basic.Error _error;
    private Dictionary<int, sg.protocol.user.UserChapter> _m_UserChapter;

    public sg.protocol.basic.Error Error
    {
      get
      {
        return _error;
      }
      set
      {
        __isset.error = true;
        this._error = value;
      }
    }

    public Dictionary<int, sg.protocol.user.UserChapter> M_UserChapter
    {
      get
      {
        return _m_UserChapter;
      }
      set
      {
        __isset.m_UserChapter = true;
        this._m_UserChapter = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool m_UserChapter;
    }

    public MsgAnsGetUserGameChapter() {
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.Struct) {
              Error = new sg.protocol.basic.Error();
              Error.Read(iprot);
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Map) {
              {
                M_UserChapter = new Dictionary<int, sg.protocol.user.UserChapter>();
                TMap _map45 = iprot.ReadMapBegin();
                for( int _i46 = 0; _i46 < _map45.Count; ++_i46)
                {
                  int _key47;
                  sg.protocol.user.UserChapter _val48;
                  _key47 = iprot.ReadI32();
                  _val48 = new sg.protocol.user.UserChapter();
                  _val48.Read(iprot);
                  M_UserChapter[_key47] = _val48;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("MsgAnsGetUserGameChapter");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Error != null && __isset.error) {
        field.Name = "error";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        Error.Write(oprot);
        oprot.WriteFieldEnd();
      }
      if (M_UserChapter != null && __isset.m_UserChapter) {
        field.Name = "m_UserChapter";
        field.Type = TType.Map;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, M_UserChapter.Count));
          foreach (int _iter49 in M_UserChapter.Keys)
          {
            oprot.WriteI32(_iter49);
            M_UserChapter[_iter49].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsGetUserGameChapter(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",M_UserChapter: ");
      sb.Append(M_UserChapter);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
