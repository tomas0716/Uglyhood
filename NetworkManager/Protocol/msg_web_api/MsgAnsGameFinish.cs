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
  public partial class MsgAnsGameFinish : TBase
  {
    private sg.protocol.basic.Error _error;
    private int _i_Ap;
    private int _i_DestroyColonyReward;
    private Dictionary<int, sg.protocol.common.GameReward> _m_GameReward;

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

    public int I_Ap
    {
      get
      {
        return _i_Ap;
      }
      set
      {
        __isset.i_Ap = true;
        this._i_Ap = value;
      }
    }

    public int I_DestroyColonyReward
    {
      get
      {
        return _i_DestroyColonyReward;
      }
      set
      {
        __isset.i_DestroyColonyReward = true;
        this._i_DestroyColonyReward = value;
      }
    }

    public Dictionary<int, sg.protocol.common.GameReward> M_GameReward
    {
      get
      {
        return _m_GameReward;
      }
      set
      {
        __isset.m_GameReward = true;
        this._m_GameReward = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool i_Ap;
      public bool i_DestroyColonyReward;
      public bool m_GameReward;
    }

    public MsgAnsGameFinish() {
      this._i_Ap = 0;
      this.__isset.i_Ap = true;
      this._i_DestroyColonyReward = 0;
      this.__isset.i_DestroyColonyReward = true;
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
            if (field.Type == TType.I32) {
              I_Ap = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_DestroyColonyReward = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Map) {
              {
                M_GameReward = new Dictionary<int, sg.protocol.common.GameReward>();
                TMap _map50 = iprot.ReadMapBegin();
                for( int _i51 = 0; _i51 < _map50.Count; ++_i51)
                {
                  int _key52;
                  sg.protocol.common.GameReward _val53;
                  _key52 = iprot.ReadI32();
                  _val53 = new sg.protocol.common.GameReward();
                  _val53.Read(iprot);
                  M_GameReward[_key52] = _val53;
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
      TStruct struc = new TStruct("MsgAnsGameFinish");
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
      if (__isset.i_Ap) {
        field.Name = "i_Ap";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Ap);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_DestroyColonyReward) {
        field.Name = "i_DestroyColonyReward";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_DestroyColonyReward);
        oprot.WriteFieldEnd();
      }
      if (M_GameReward != null && __isset.m_GameReward) {
        field.Name = "m_GameReward";
        field.Type = TType.Map;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, M_GameReward.Count));
          foreach (int _iter54 in M_GameReward.Keys)
          {
            oprot.WriteI32(_iter54);
            M_GameReward[_iter54].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsGameFinish(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",I_Ap: ");
      sb.Append(I_Ap);
      sb.Append(",I_DestroyColonyReward: ");
      sb.Append(I_DestroyColonyReward);
      sb.Append(",M_GameReward: ");
      sb.Append(M_GameReward);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
