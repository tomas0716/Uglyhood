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

namespace sg.protocol.common
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class ConnectReward : TBase
  {
    private int _i_RewardSeq;
    private int _i_RewardKind;
    private int _i_RewardId;
    private int _i_RewardCount;

    public int I_RewardSeq
    {
      get
      {
        return _i_RewardSeq;
      }
      set
      {
        __isset.i_RewardSeq = true;
        this._i_RewardSeq = value;
      }
    }

    public int I_RewardKind
    {
      get
      {
        return _i_RewardKind;
      }
      set
      {
        __isset.i_RewardKind = true;
        this._i_RewardKind = value;
      }
    }

    public int I_RewardId
    {
      get
      {
        return _i_RewardId;
      }
      set
      {
        __isset.i_RewardId = true;
        this._i_RewardId = value;
      }
    }

    public int I_RewardCount
    {
      get
      {
        return _i_RewardCount;
      }
      set
      {
        __isset.i_RewardCount = true;
        this._i_RewardCount = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_RewardSeq;
      public bool i_RewardKind;
      public bool i_RewardId;
      public bool i_RewardCount;
    }

    public ConnectReward() {
      this._i_RewardSeq = 0;
      this.__isset.i_RewardSeq = true;
      this._i_RewardKind = 0;
      this.__isset.i_RewardKind = true;
      this._i_RewardId = 0;
      this.__isset.i_RewardId = true;
      this._i_RewardCount = 0;
      this.__isset.i_RewardCount = true;
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
            if (field.Type == TType.I32) {
              I_RewardSeq = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_RewardKind = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_RewardId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_RewardCount = iprot.ReadI32();
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
      TStruct struc = new TStruct("ConnectReward");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_RewardSeq) {
        field.Name = "i_RewardSeq";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardSeq);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardKind) {
        field.Name = "i_RewardKind";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardKind);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId) {
        field.Name = "i_RewardId";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount) {
        field.Name = "i_RewardCount";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("ConnectReward(");
      sb.Append("I_RewardSeq: ");
      sb.Append(I_RewardSeq);
      sb.Append(",I_RewardKind: ");
      sb.Append(I_RewardKind);
      sb.Append(",I_RewardId: ");
      sb.Append(I_RewardId);
      sb.Append(",I_RewardCount: ");
      sb.Append(I_RewardCount);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
