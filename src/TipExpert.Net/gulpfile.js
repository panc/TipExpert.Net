/// <binding BeforeBuild='min, inject' Clean='clean' />
var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    project = require("./project.json"),
    inject = require('gulp-inject');

var paths = {
    webroot: "./" + project.webroot + "/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);


gulp.task("min:js", function () {
    gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);


gulp.task('inject:js', function () {
    // it's not necessary to read the files (will speed up things), we're only after their paths: 
    var sources = gulp.src([paths.js, "!" + paths.minJs], { read: false });

    var options = {
        ignorePath: '/wwwroot',
        addPrefix: '~',
        addRootSlash: false
    };

    return gulp.src('./Views/Shared/_Layout.cshtml')
        .pipe(inject(sources, options))
        .pipe(gulp.dest('./Views/Shared/'));
});

gulp.task('inject:css', function () {
    //tbd
});

gulp.task("inject", ["inject:js", "inject:css"]);
