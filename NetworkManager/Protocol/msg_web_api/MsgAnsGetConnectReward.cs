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
  public partial class MsgAnsGetConnectReward : TBase
  {
    private sg.protocol.basic.Error _error;
    private Dictionary<int, sg.protocol.common.ConnectReward> _m_ConnectReward;

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

    public Dictionary<int, sg.protocol.common.ConnectReward> M_ConnectReward
    {
      get
      {
        return _m_ConnectReward;
      }
      set
      {
        __isset.m_ConnectReward = true;
        this._m_ConnectReward = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool m_ConnectReward;
    }

    public MsgAnsGetConnectReward() {
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
                M_ConnectReward = new Dictionary<int, sg.protocol.common.ConnectReward>();
                TMap _map30 = iprot.ReadMapBegin();
                for( int _i31 = 0; _i31 < _map30.Count; ++_i31)
                {
                  int _key32;
                  sg.protocol.common.ConnectReward _val33;
                  _key32 = iprot.ReadI32();
                  _val33 = new sg.protocol.common.ConnectReward();
                  _val33.Read(iprot);
                  M_ConnectReward[_key32] = _val33;
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
      TStruct struc = new TStruct("MsgAnsGetConnectReward");
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
      if (M_ConnectReward != null && __isset.m_ConnectReward) {
        field.Name = "m_ConnectReward";
        field.Type = TType.Map;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, M_ConnectReward.Count));
          foreach (int _iter34 in M_ConnectReward.Keys)
          {
            oprot.WriteI32(_iter34);
            M_ConnectReward[_iter34].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsGetConnectReward(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",M_ConnectReward: ");
      sb.Append(M_ConnectReward);
      sb.Append(")");
      return sb.ToString();
    }

  }

}