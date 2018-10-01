package sdonjava;

import sdonjava.serialization.SDONSerializer;
import sdonjava.model.Diagram;

import com.google.gson.*;

public abstract class SDONBuilder {
    public static String ToJSON(Diagram diagram) {
        SDONSerializer serializer = new SDONSerializer();
        return serializer.SerializeObject(diagram);
    }
    
    public static Diagram FromJSON(String json) {
        Gson serial = new Gson();
        return serial.fromJson(json, Diagram.class);
    }
}