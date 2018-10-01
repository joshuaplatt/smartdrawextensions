package sdonjava.serialization;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

@Retention(RetentionPolicy.RUNTIME)
public @interface SDONSerializable {
    int defaultInt() default -1;
    double defaultDouble() default -1.0;
}