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
  public partial class UserChapter : TBase
  {
    private int _i_Seq;
    private int _i_EpisodeId;
    private int _i_ChapterId;
    private int _i_StageId;
    private int _i_IsClear;
    private int _i_StarCount;
    private int _i_MissionCount;

    public int I_Seq
    {
      get
      {
        return _i_Seq;
      }
      set
      {
        __isset.i_Seq = true;
        this._i_Seq = value;
      }
    }

    public int I_EpisodeId
    {
      get
      {
        return _i_EpisodeId;
      }
      set
      {
        __isset.i_EpisodeId = true;
        this._i_EpisodeId = value;
      }
    }

    public int I_ChapterId
    {
      get
      {
        return _i_ChapterId;
      }
      set
      {
        __isset.i_ChapterId = true;
        this._i_ChapterId = value;
      }
    }

    public int I_StageId
    {
      get
      {
        return _i_StageId;
      }
      set
      {
        __isset.i_StageId = true;
        this._i_StageId = value;
      }
    }

    public int I_IsClear
    {
      get
      {
        return _i_IsClear;
      }
      set
      {
        __isset.i_IsClear = true;
        this._i_IsClear = value;
      }
    }

    public int I_StarCount
    {
      get
      {
        return _i_StarCount;
      }
      set
      {
        __isset.i_StarCount = true;
        this._i_StarCount = value;
      }
    }

    public int I_MissionCount
    {
      get
      {
        return _i_MissionCount;
      }
      set
      {
        __isset.i_MissionCount = true;
        this._i_MissionCount = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_Seq;
      public bool i_EpisodeId;
      public bool i_ChapterId;
      public bool i_StageId;
      public bool i_IsClear;
      public bool i_StarCount;
      public bool i_MissionCount;
    }

    public UserChapter() {
      this._i_Seq = 0;
      this.__isset.i_Seq = true;
      this._i_EpisodeId = 0;
      this.__isset.i_EpisodeId = true;
      this._i_ChapterId = 0;
      this.__isset.i_ChapterId = true;
      this._i_StageId = 0;
      this.__isset.i_StageId = true;
      this._i_IsClear = 0;
      this.__isset.i_IsClear = true;
      this._i_StarCount = 0;
      this.__isset.i_StarCount = true;
      this._i_MissionCount = 0;
      this.__isset.i_MissionCount = true;
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
              I_Seq = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_EpisodeId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_ChapterId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_StageId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              I_IsClear = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I32) {
              I_StarCount = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              I_MissionCount = iprot.ReadI32();
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
      TStruct struc = new TStruct("UserChapter");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_Seq) {
        field.Name = "i_Seq";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Seq);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_EpisodeId) {
        field.Name = "i_EpisodeId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_EpisodeId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ChapterId) {
        field.Name = "i_ChapterId";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ChapterId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_StageId) {
        field.Name = "i_StageId";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_StageId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_IsClear) {
        field.Name = "i_IsClear";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_IsClear);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_StarCount) {
        field.Name = "i_StarCount";
        field.Type = TType.I32;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_StarCount);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_MissionCount) {
        field.Name = "i_MissionCount";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_MissionCount);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("UserChapter(");
      sb.Append("I_Seq: ");
      sb.Append(I_Seq);
      sb.Append(",I_EpisodeId: ");
      sb.Append(I_EpisodeId);
      sb.Append(",I_ChapterId: ");
      sb.Append(I_ChapterId);
      sb.Append(",I_StageId: ");
      sb.Append(I_StageId);
      sb.Append(",I_IsClear: ");
      sb.Append(I_IsClear);
      sb.Append(",I_StarCount: ");
      sb.Append(I_StarCount);
      sb.Append(",I_MissionCount: ");
      sb.Append(I_MissionCount);
      sb.Append(")");
      return sb.ToString();
    }

  }

}