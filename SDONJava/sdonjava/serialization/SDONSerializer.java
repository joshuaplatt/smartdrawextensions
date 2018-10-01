package sdonjava.serialization;

import java.io.IOException;
import java.lang.reflect.*;
import java.util.Collection;

import com.google.gson.*;
import com.google.gson.stream.JsonReader;
import com.google.gson.stream.JsonWriter;

public class SDONSerializer {
    private Gson gson;
    
    public SDONSerializer() {
        gson = new GsonBuilder().disableHtmlEscaping().create();
    }
    
    public String SerializeObject(Object obj) {
        JsonElement root = reflectGeneric(obj, null);
        String json = gson.toJson(root);
        
        return json.replace("\\\\/", "\\/");
    }
    
    private JsonElement reflectGeneric(Object obj, Field base) {
        JsonElement el = gson.toJsonTree(null);
        
        if(base != null) {
            SDONSerializable annotation = base.getAnnotation(SDONSerializable.class);
            
            if(annotation == null) {
                return gson.toJsonTree(null);
            }
        }
        
        if(obj instanceof Integer) {
            el = getInteger(((Integer)obj).intValue(), base);
        }
        else if(obj instanceof Double) {
            el = getDouble(((Double)obj).doubleValue(), base);
        }
        else if(obj instanceof Collection<?>) {
            el = getArray((Collection<?>)obj);
        }
        else if(obj instanceof Boolean) {
            el = getBoolean((Boolean)obj);
        }
        else if(obj instanceof String) {
            el = getString((String)obj);
        }
        else {
            el = getObject(obj);
        }
        
        return el;
    }
    
    private JsonElement getArray(Collection<?> arr) {
        if(arr.size() == 0) {
            return gson.toJsonTree(null);
        }
        
        JsonArray ret = new JsonArray();
        
        for(Object obj : arr) {
            ret.add(reflectGeneric(obj, null));
        }
        
        return ret;
    }
    
    private JsonElement getDouble(double val, Field base) {
        if(base != null) {
            SDONSerializable annotation = base.getAnnotation(SDONSerializable.class);
            
            if((annotation != null) && (annotation.defaultDouble() == val)) {
                return gson.toJsonTree(null);
            }
        }
        
        String testStr = Double.toString(val);
        if(testStr.matches("^-?[0-9]+\\.?0*$")) {
            //Could be represented without a decimal
            return gson.toJsonTree((int)val);
        }
        
        return gson.toJsonTree(val);
    }
    
    private JsonElement getInteger(int val, Field base) {
        if(base != null) {
            SDONSerializable annotation = base.getAnnotation(SDONSerializable.class);
            
            if((annotation != null) && (annotation.defaultInt() == val)) {
                return gson.toJsonTree(null);
            }
        }
        
        return gson.toJsonTree(val);
    }
    
    private JsonElement getBoolean(Boolean val) {
        return gson.toJsonTree(val);
    }
    
    private JsonElement getString(String val) {
        if(val.length() == 0) {
            return gson.toJsonTree(null);
        }
        
        if(val.indexOf('/') >= 0) {
            return gson.toJsonTree(val.replace("/", "\\/"));
        }
        
        return gson.toJsonTree(val);
    }
    
    private JsonElement getObject(Object obj) {
        if(obj == null) {
            return gson.toJsonTree(null);
        }
        
        Field[] fields = obj.getClass().getDeclaredFields();
        JsonObject ret = new JsonObject();
        JsonElement temp;
        boolean hasNonNull = false;
        
        for(Field f : fields) {
            try {
                temp = reflectGeneric(f.get(obj), f);
                ret.add(f.getName(), temp);
                
                if(!temp.isJsonNull()) {
                    hasNonNull = true;
                }
            }
            catch(IllegalAccessException e) {
                return gson.toJsonTree(null);
            }
        }
        
        if(!hasNonNull) {
            return gson.toJsonTree(null);
        }
        
        return ret;
    }
}