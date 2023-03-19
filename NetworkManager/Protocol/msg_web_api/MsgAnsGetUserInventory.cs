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
  public partial class MsgAnsGetUserInventory : TBase
  {
    private sg.protocol.basic.Error _error;
    private Dictionary<int, sg.protocol.user.UserItem> _m_UserItem;

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

    public Dictionary<int, sg.protocol.user.UserItem> M_UserItem
    {
      get
      {
        return _m_UserItem;
      }
      set
      {
        __isset.m_UserItem = true;
        this._m_UserItem = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool m_UserItem;
    }

    public MsgAnsGetUserInventory() {
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
                M_UserItem = new Dictionary<int, sg.protocol.user.UserItem>();
                TMap _map20 = iprot.ReadMapBegin();
                for( int _i21 = 0; _i21 < _map20.Count; ++_i21)
                {
                  int _key22;
                  sg.protocol.user.UserItem _val23;
                  _key22 = iprot.ReadI32();
                  _val23 = new sg.protocol.user.UserItem();
                  _val23.Read(iprot);
                  M_UserItem[_key22] = _val23;
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
      TStruct struc = new TStruct("MsgAnsGetUserInventory");
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
      if (M_UserItem != null && __isset.m_UserItem) {
        field.Name = "m_UserItem";
        field.Type = TType.Map;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, M_UserItem.Count));
          foreach (int _iter24 in M_UserItem.Keys)
          {
            oprot.WriteI32(_iter24);
            M_UserItem[_iter24].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsGetUserInventory(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",M_UserItem: ");
      sb.Append(M_UserItem);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
