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
  public partial class SpecialStageList : TBase
  {
    private int _i_ModeId;
    private int _i_AvailablePlayerRank;
    private int _i_ScheduleType;
    private int _i_ScheduleDayStart;
    private int _i_ScheduleDayEnd;
    private int _i_ScheduleTimeStart;
    private int _i_ScheduleTimeEnd;

    public int I_ModeId
    {
      get
      {
        return _i_ModeId;
      }
      set
      {
        __isset.i_ModeId = true;
        this._i_ModeId = value;
      }
    }

    public int I_AvailablePlayerRank
    {
      get
      {
        return _i_AvailablePlayerRank;
      }
      set
      {
        __isset.i_AvailablePlayerRank = true;
        this._i_AvailablePlayerRank = value;
      }
    }

    public int I_ScheduleType
    {
      get
      {
        return _i_ScheduleType;
      }
      set
      {
        __isset.i_ScheduleType = true;
        this._i_ScheduleType = value;
      }
    }

    public int I_ScheduleDayStart
    {
      get
      {
        return _i_ScheduleDayStart;
      }
      set
      {
        __isset.i_ScheduleDayStart = true;
        this._i_ScheduleDayStart = value;
      }
    }

    public int I_ScheduleDayEnd
    {
      get
      {
        return _i_ScheduleDayEnd;
      }
      set
      {
        __isset.i_ScheduleDayEnd = true;
        this._i_ScheduleDayEnd = value;
      }
    }

    public int I_ScheduleTimeStart
    {
      get
      {
        return _i_ScheduleTimeStart;
      }
      set
      {
        __isset.i_ScheduleTimeStart = true;
        this._i_ScheduleTimeStart = value;
      }
    }

    public int I_ScheduleTimeEnd
    {
      get
      {
        return _i_ScheduleTimeEnd;
      }
      set
      {
        __isset.i_ScheduleTimeEnd = true;
        this._i_ScheduleTimeEnd = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_ModeId;
      public bool i_AvailablePlayerRank;
      public bool i_ScheduleType;
      public bool i_ScheduleDayStart;
      public bool i_ScheduleDayEnd;
      public bool i_ScheduleTimeStart;
      public bool i_ScheduleTimeEnd;
    }

    public SpecialStageList() {
      this._i_ModeId = 0;
      this.__isset.i_ModeId = true;
      this._i_AvailablePlayerRank = 0;
      this.__isset.i_AvailablePlayerRank = true;
      this._i_ScheduleType = 0;
      this.__isset.i_ScheduleType = true;
      this._i_ScheduleDayStart = 0;
      this.__isset.i_ScheduleDayStart = true;
      this._i_ScheduleDayEnd = 0;
      this.__isset.i_ScheduleDayEnd = true;
      this._i_ScheduleTimeStart = 0;
      this.__isset.i_ScheduleTimeStart = true;
      this._i_ScheduleTimeEnd = 0;
      this.__isset.i_ScheduleTimeEnd = true;
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
              I_ModeId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_AvailablePlayerRank = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_ScheduleType = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_ScheduleDayStart = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              I_ScheduleDayEnd = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I32) {
              I_ScheduleTimeStart = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              I_ScheduleTimeEnd = iprot.ReadI32();
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
      TStruct struc = new TStruct("SpecialStageList");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_ModeId) {
        field.Name = "i_ModeId";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ModeId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_AvailablePlayerRank) {
        field.Name = "i_AvailablePlayerRank";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_AvailablePlayerRank);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ScheduleType) {
        field.Name = "i_ScheduleType";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ScheduleType);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ScheduleDayStart) {
        field.Name = "i_ScheduleDayStart";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ScheduleDayStart);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ScheduleDayEnd) {
        field.Name = "i_ScheduleDayEnd";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ScheduleDayEnd);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ScheduleTimeStart) {
        field.Name = "i_ScheduleTimeStart";
        field.Type = TType.I32;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ScheduleTimeStart);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ScheduleTimeEnd) {
        field.Name = "i_ScheduleTimeEnd";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ScheduleTimeEnd);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("SpecialStageList(");
      sb.Append("I_ModeId: ");
      sb.Append(I_ModeId);
      sb.Append(",I_AvailablePlayerRank: ");
      sb.Append(I_AvailablePlayerRank);
      sb.Append(",I_ScheduleType: ");
      sb.Append(I_ScheduleType);
      sb.Append(",I_ScheduleDayStart: ");
      sb.Append(I_ScheduleDayStart);
      sb.Append(",I_ScheduleDayEnd: ");
      sb.Append(I_ScheduleDayEnd);
      sb.Append(",I_ScheduleTimeStart: ");
      sb.Append(I_ScheduleTimeStart);
      sb.Append(",I_ScheduleTimeEnd: ");
      sb.Append(I_ScheduleTimeEnd);
      sb.Append(")");
      return sb.ToString();
    }

  }

}