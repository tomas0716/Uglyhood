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
  public partial class MsgAnsUserCheckAttendance : TBase
  {
    private sg.protocol.basic.Error _error;
    private sg.protocol.common.Attendance _st_Attendance;

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

    public sg.protocol.common.Attendance St_Attendance
    {
      get
      {
        return _st_Attendance;
      }
      set
      {
        __isset.st_Attendance = true;
        this._st_Attendance = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool st_Attendance;
    }

    public MsgAnsUserCheckAttendance() {
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
            if (field.Type == TType.Struct) {
              St_Attendance = new sg.protocol.common.Attendance();
              St_Attendance.Read(iprot);
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
      TStruct struc = new TStruct("MsgAnsUserCheckAttendance");
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
      if (St_Attendance != null && __isset.st_Attendance) {
        field.Name = "st_Attendance";
        field.Type = TType.Struct;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        St_Attendance.Write(oprot);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsUserCheckAttendance(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",St_Attendance: ");
      sb.Append(St_Attendance== null ? "<null>" : St_Attendance.ToString());
      sb.Append(")");
      return sb.ToString();
    }

  }

}