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

namespace sg.protocol.user
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class UserEvent : TBase
  {
    private int _i_EventSeq;
    private int _i_EventId;
    private int _i_UserCount;
    private bool _b_IsReward;
    private bool _b_IsComplete;

    public int I_EventSeq
    {
      get
      {
        return _i_EventSeq;
      }
      set
      {
        __isset.i_EventSeq = true;
        this._i_EventSeq = value;
      }
    }

    public int I_EventId
    {
      get
      {
        return _i_EventId;
      }
      set
      {
        __isset.i_EventId = true;
        this._i_EventId = value;
      }
    }

    public int I_UserCount
    {
      get
      {
        return _i_UserCount;
      }
      set
      {
        __isset.i_UserCount = true;
        this._i_UserCount = value;
      }
    }

    public bool B_IsReward
    {
      get
      {
        return _b_IsReward;
      }
      set
      {
        __isset.b_IsReward = true;
        this._b_IsReward = value;
      }
    }

    public bool B_IsComplete
    {
      get
      {
        return _b_IsComplete;
      }
      set
      {
        __isset.b_IsComplete = true;
        this._b_IsComplete = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_EventSeq;
      public bool i_EventId;
      public bool i_UserCount;
      public bool b_IsReward;
      public bool b_IsComplete;
    }

    public UserEvent() {
      this._i_EventSeq = 0;
      this.__isset.i_EventSeq = true;
      this._i_EventId = 0;
      this.__isset.i_EventId = true;
      this._i_UserCount = 0;
      this.__isset.i_UserCount = true;
      this._b_IsReward = false;
      this.__isset.b_IsReward = true;
      this._b_IsComplete = false;
      this.__isset.b_IsComplete = true;
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
              I_EventSeq = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_EventId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_UserCount = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Bool) {
              B_IsReward = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.Bool) {
              B_IsComplete = iprot.ReadBool();
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
      TStruct struc = new TStruct("UserEvent");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_EventSeq) {
        field.Name = "i_EventSeq";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_EventSeq);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_EventId) {
        field.Name = "i_EventId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_EventId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_UserCount) {
        field.Name = "i_UserCount";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_UserCount);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsReward) {
        field.Name = "b_IsReward";
        field.Type = TType.Bool;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsReward);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsComplete) {
        field.Name = "b_IsComplete";
        field.Type = TType.Bool;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsComplete);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("UserEvent(");
      sb.Append("I_EventSeq: ");
      sb.Append(I_EventSeq);
      sb.Append(",I_EventId: ");
      sb.Append(I_EventId);
      sb.Append(",I_UserCount: ");
      sb.Append(I_UserCount);
      sb.Append(",B_IsReward: ");
      sb.Append(B_IsReward);
      sb.Append(",B_IsComplete: ");
      sb.Append(B_IsComplete);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
