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
  public partial class MsgAnsUserInfoOfflineReward : TBase
  {
    private sg.protocol.basic.Error _error;
    private int _i_RewardGold;
    private int _i_RewardId_1;
    private int _i_RewardCount_1;
    private int _i_RewardId_2;
    private int _i_RewardCount_2;
    private int _i_RewardId_3;
    private int _i_RewardCount_3;
    private int _i_RewardId_4;
    private int _i_RewardCount_4;
    private int _i_RewardId_5;
    private int _i_RewardCount_5;
    private int _i_RewardId_6;
    private int _i_RewardCount_6;
    private int _i_RewardId_7;
    private int _i_RewardCount_7;
    private int _i_RewardId_8;
    private int _i_RewardCount_8;
    private int _i_RewardId_9;
    private int _i_RewardCount_9;
    private int _i_AccrueTime;
    private int _i_FastAdLimit;
    private int _i_FastGemLimit;
    private int _i_NextStartTime;

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

    public int I_RewardGold
    {
      get
      {
        return _i_RewardGold;
      }
      set
      {
        __isset.i_RewardGold = true;
        this._i_RewardGold = value;
      }
    }

    public int I_RewardId_1
    {
      get
      {
        return _i_RewardId_1;
      }
      set
      {
        __isset.i_RewardId_1 = true;
        this._i_RewardId_1 = value;
      }
    }

    public int I_RewardCount_1
    {
      get
      {
        return _i_RewardCount_1;
      }
      set
      {
        __isset.i_RewardCount_1 = true;
        this._i_RewardCount_1 = value;
      }
    }

    public int I_RewardId_2
    {
      get
      {
        return _i_RewardId_2;
      }
      set
      {
        __isset.i_RewardId_2 = true;
        this._i_RewardId_2 = value;
      }
    }

    public int I_RewardCount_2
    {
      get
      {
        return _i_RewardCount_2;
      }
      set
      {
        __isset.i_RewardCount_2 = true;
        this._i_RewardCount_2 = value;
      }
    }

    public int I_RewardId_3
    {
      get
      {
        return _i_RewardId_3;
      }
      set
      {
        __isset.i_RewardId_3 = true;
        this._i_RewardId_3 = value;
      }
    }

    public int I_RewardCount_3
    {
      get
      {
        return _i_RewardCount_3;
      }
      set
      {
        __isset.i_RewardCount_3 = true;
        this._i_RewardCount_3 = value;
      }
    }

    public int I_RewardId_4
    {
      get
      {
        return _i_RewardId_4;
      }
      set
      {
        __isset.i_RewardId_4 = true;
        this._i_RewardId_4 = value;
      }
    }

    public int I_RewardCount_4
    {
      get
      {
        return _i_RewardCount_4;
      }
      set
      {
        __isset.i_RewardCount_4 = true;
        this._i_RewardCount_4 = value;
      }
    }

    public int I_RewardId_5
    {
      get
      {
        return _i_RewardId_5;
      }
      set
      {
        __isset.i_RewardId_5 = true;
        this._i_RewardId_5 = value;
      }
    }

    public int I_RewardCount_5
    {
      get
      {
        return _i_RewardCount_5;
      }
      set
      {
        __isset.i_RewardCount_5 = true;
        this._i_RewardCount_5 = value;
      }
    }

    public int I_RewardId_6
    {
      get
      {
        return _i_RewardId_6;
      }
      set
      {
        __isset.i_RewardId_6 = true;
        this._i_RewardId_6 = value;
      }
    }

    public int I_RewardCount_6
    {
      get
      {
        return _i_RewardCount_6;
      }
      set
      {
        __isset.i_RewardCount_6 = true;
        this._i_RewardCount_6 = value;
      }
    }

    public int I_RewardId_7
    {
      get
      {
        return _i_RewardId_7;
      }
      set
      {
        __isset.i_RewardId_7 = true;
        this._i_RewardId_7 = value;
      }
    }

    public int I_RewardCount_7
    {
      get
      {
        return _i_RewardCount_7;
      }
      set
      {
        __isset.i_RewardCount_7 = true;
        this._i_RewardCount_7 = value;
      }
    }

    public int I_RewardId_8
    {
      get
      {
        return _i_RewardId_8;
      }
      set
      {
        __isset.i_RewardId_8 = true;
        this._i_RewardId_8 = value;
      }
    }

    public int I_RewardCount_8
    {
      get
      {
        return _i_RewardCount_8;
      }
      set
      {
        __isset.i_RewardCount_8 = true;
        this._i_RewardCount_8 = value;
      }
    }

    public int I_RewardId_9
    {
      get
      {
        return _i_RewardId_9;
      }
      set
      {
        __isset.i_RewardId_9 = true;
        this._i_RewardId_9 = value;
      }
    }

    public int I_RewardCount_9
    {
      get
      {
        return _i_RewardCount_9;
      }
      set
      {
        __isset.i_RewardCount_9 = true;
        this._i_RewardCount_9 = value;
      }
    }

    public int I_AccrueTime
    {
      get
      {
        return _i_AccrueTime;
      }
      set
      {
        __isset.i_AccrueTime = true;
        this._i_AccrueTime = value;
      }
    }

    public int I_FastAdLimit
    {
      get
      {
        return _i_FastAdLimit;
      }
      set
      {
        __isset.i_FastAdLimit = true;
        this._i_FastAdLimit = value;
      }
    }

    public int I_FastGemLimit
    {
      get
      {
        return _i_FastGemLimit;
      }
      set
      {
        __isset.i_FastGemLimit = true;
        this._i_FastGemLimit = value;
      }
    }

    public int I_NextStartTime
    {
      get
      {
        return _i_NextStartTime;
      }
      set
      {
        __isset.i_NextStartTime = true;
        this._i_NextStartTime = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool i_RewardGold;
      public bool i_RewardId_1;
      public bool i_RewardCount_1;
      public bool i_RewardId_2;
      public bool i_RewardCount_2;
      public bool i_RewardId_3;
      public bool i_RewardCount_3;
      public bool i_RewardId_4;
      public bool i_RewardCount_4;
      public bool i_RewardId_5;
      public bool i_RewardCount_5;
      public bool i_RewardId_6;
      public bool i_RewardCount_6;
      public bool i_RewardId_7;
      public bool i_RewardCount_7;
      public bool i_RewardId_8;
      public bool i_RewardCount_8;
      public bool i_RewardId_9;
      public bool i_RewardCount_9;
      public bool i_AccrueTime;
      public bool i_FastAdLimit;
      public bool i_FastGemLimit;
      public bool i_NextStartTime;
    }

    public MsgAnsUserInfoOfflineReward() {
      this._i_RewardGold = 0;
      this.__isset.i_RewardGold = true;
      this._i_RewardId_1 = 0;
      this.__isset.i_RewardId_1 = true;
      this._i_RewardCount_1 = 0;
      this.__isset.i_RewardCount_1 = true;
      this._i_RewardId_2 = 0;
      this.__isset.i_RewardId_2 = true;
      this._i_RewardCount_2 = 0;
      this.__isset.i_RewardCount_2 = true;
      this._i_RewardId_3 = 0;
      this.__isset.i_RewardId_3 = true;
      this._i_RewardCount_3 = 0;
      this.__isset.i_RewardCount_3 = true;
      this._i_RewardId_4 = 0;
      this.__isset.i_RewardId_4 = true;
      this._i_RewardCount_4 = 0;
      this.__isset.i_RewardCount_4 = true;
      this._i_RewardId_5 = 0;
      this.__isset.i_RewardId_5 = true;
      this._i_RewardCount_5 = 0;
      this.__isset.i_RewardCount_5 = true;
      this._i_RewardId_6 = 0;
      this.__isset.i_RewardId_6 = true;
      this._i_RewardCount_6 = 0;
      this.__isset.i_RewardCount_6 = true;
      this._i_RewardId_7 = 0;
      this.__isset.i_RewardId_7 = true;
      this._i_RewardCount_7 = 0;
      this.__isset.i_RewardCount_7 = true;
      this._i_RewardId_8 = 0;
      this.__isset.i_RewardId_8 = true;
      this._i_RewardCount_8 = 0;
      this.__isset.i_RewardCount_8 = true;
      this._i_RewardId_9 = 0;
      this.__isset.i_RewardId_9 = true;
      this._i_RewardCount_9 = 0;
      this.__isset.i_RewardCount_9 = true;
      this._i_AccrueTime = 0;
      this.__isset.i_AccrueTime = true;
      this._i_FastAdLimit = 0;
      this.__isset.i_FastAdLimit = true;
      this._i_FastGemLimit = 0;
      this.__isset.i_FastGemLimit = true;
      this._i_NextStartTime = 0;
      this.__isset.i_NextStartTime = true;
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
              I_RewardGold = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_RewardId_1 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_RewardCount_1 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              I_RewardId_2 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I32) {
              I_RewardCount_2 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              I_RewardId_3 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.I32) {
              I_RewardCount_3 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 9:
            if (field.Type == TType.I32) {
              I_RewardId_4 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 10:
            if (field.Type == TType.I32) {
              I_RewardCount_4 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 11:
            if (field.Type == TType.I32) {
              I_RewardId_5 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 12:
            if (field.Type == TType.I32) {
              I_RewardCount_5 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 13:
            if (field.Type == TType.I32) {
              I_RewardId_6 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 14:
            if (field.Type == TType.I32) {
              I_RewardCount_6 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 15:
            if (field.Type == TType.I32) {
              I_RewardId_7 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 16:
            if (field.Type == TType.I32) {
              I_RewardCount_7 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 17:
            if (field.Type == TType.I32) {
              I_RewardId_8 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 18:
            if (field.Type == TType.I32) {
              I_RewardCount_8 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 19:
            if (field.Type == TType.I32) {
              I_RewardId_9 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 20:
            if (field.Type == TType.I32) {
              I_RewardCount_9 = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 21:
            if (field.Type == TType.I32) {
              I_AccrueTime = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 22:
            if (field.Type == TType.I32) {
              I_FastAdLimit = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 23:
            if (field.Type == TType.I32) {
              I_FastGemLimit = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 24:
            if (field.Type == TType.I32) {
              I_NextStartTime = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgAnsUserInfoOfflineReward");
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
      if (__isset.i_RewardGold) {
        field.Name = "i_RewardGold";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardGold);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_1) {
        field.Name = "i_RewardId_1";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_1);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_1) {
        field.Name = "i_RewardCount_1";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_1);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_2) {
        field.Name = "i_RewardId_2";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_2);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_2) {
        field.Name = "i_RewardCount_2";
        field.Type = TType.I32;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_2);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_3) {
        field.Name = "i_RewardId_3";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_3);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_3) {
        field.Name = "i_RewardCount_3";
        field.Type = TType.I32;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_3);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_4) {
        field.Name = "i_RewardId_4";
        field.Type = TType.I32;
        field.ID = 9;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_4);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_4) {
        field.Name = "i_RewardCount_4";
        field.Type = TType.I32;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_4);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_5) {
        field.Name = "i_RewardId_5";
        field.Type = TType.I32;
        field.ID = 11;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_5);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_5) {
        field.Name = "i_RewardCount_5";
        field.Type = TType.I32;
        field.ID = 12;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_5);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_6) {
        field.Name = "i_RewardId_6";
        field.Type = TType.I32;
        field.ID = 13;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_6);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_6) {
        field.Name = "i_RewardCount_6";
        field.Type = TType.I32;
        field.ID = 14;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_6);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_7) {
        field.Name = "i_RewardId_7";
        field.Type = TType.I32;
        field.ID = 15;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_7);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_7) {
        field.Name = "i_RewardCount_7";
        field.Type = TType.I32;
        field.ID = 16;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_7);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_8) {
        field.Name = "i_RewardId_8";
        field.Type = TType.I32;
        field.ID = 17;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_8);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_8) {
        field.Name = "i_RewardCount_8";
        field.Type = TType.I32;
        field.ID = 18;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_8);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardId_9) {
        field.Name = "i_RewardId_9";
        field.Type = TType.I32;
        field.ID = 19;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardId_9);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_RewardCount_9) {
        field.Name = "i_RewardCount_9";
        field.Type = TType.I32;
        field.ID = 20;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_RewardCount_9);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_AccrueTime) {
        field.Name = "i_AccrueTime";
        field.Type = TType.I32;
        field.ID = 21;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_AccrueTime);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_FastAdLimit) {
        field.Name = "i_FastAdLimit";
        field.Type = TType.I32;
        field.ID = 22;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_FastAdLimit);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_FastGemLimit) {
        field.Name = "i_FastGemLimit";
        field.Type = TType.I32;
        field.ID = 23;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_FastGemLimit);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_NextStartTime) {
        field.Name = "i_NextStartTime";
        field.Type = TType.I32;
        field.ID = 24;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_NextStartTime);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsUserInfoOfflineReward(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",I_RewardGold: ");
      sb.Append(I_RewardGold);
      sb.Append(",I_RewardId_1: ");
      sb.Append(I_RewardId_1);
      sb.Append(",I_RewardCount_1: ");
      sb.Append(I_RewardCount_1);
      sb.Append(",I_RewardId_2: ");
      sb.Append(I_RewardId_2);
      sb.Append(",I_RewardCount_2: ");
      sb.Append(I_RewardCount_2);
      sb.Append(",I_RewardId_3: ");
      sb.Append(I_RewardId_3);
      sb.Append(",I_RewardCount_3: ");
      sb.Append(I_RewardCount_3);
      sb.Append(",I_RewardId_4: ");
      sb.Append(I_RewardId_4);
      sb.Append(",I_RewardCount_4: ");
      sb.Append(I_RewardCount_4);
      sb.Append(",I_RewardId_5: ");
      sb.Append(I_RewardId_5);
      sb.Append(",I_RewardCount_5: ");
      sb.Append(I_RewardCount_5);
      sb.Append(",I_RewardId_6: ");
      sb.Append(I_RewardId_6);
      sb.Append(",I_RewardCount_6: ");
      sb.Append(I_RewardCount_6);
      sb.Append(",I_RewardId_7: ");
      sb.Append(I_RewardId_7);
      sb.Append(",I_RewardCount_7: ");
      sb.Append(I_RewardCount_7);
      sb.Append(",I_RewardId_8: ");
      sb.Append(I_RewardId_8);
      sb.Append(",I_RewardCount_8: ");
      sb.Append(I_RewardCount_8);
      sb.Append(",I_RewardId_9: ");
      sb.Append(I_RewardId_9);
      sb.Append(",I_RewardCount_9: ");
      sb.Append(I_RewardCount_9);
      sb.Append(",I_AccrueTime: ");
      sb.Append(I_AccrueTime);
      sb.Append(",I_FastAdLimit: ");
      sb.Append(I_FastAdLimit);
      sb.Append(",I_FastGemLimit: ");
      sb.Append(I_FastGemLimit);
      sb.Append(",I_NextStartTime: ");
      sb.Append(I_NextStartTime);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
